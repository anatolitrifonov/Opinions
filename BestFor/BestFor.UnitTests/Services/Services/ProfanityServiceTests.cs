using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.Services;
using BestFor.UnitTests.Testables;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Services.Services
{
    [ExcludeFromCodeCoverage]
    public class ProfanityServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public ProfanityService ProfanityService;
            public Mock<IResourcesService> ResourcesServiceMock;
            public Mock<ICacheManager> CacheMock;
            public Repository<BadWord> BadWordsRepository;
            public TestLoggerFactory TestLoggerFactory;
            public const string ANY_RESOURCE_STRING = "a";

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                BadWordsRepository = new Repository<BadWord>(dataContext);
                CacheMock = new TestCacheManager().CacheMock;
                ResourcesServiceMock = new Mock<IResourcesService>();
                ResourcesServiceMock.Setup(x => x.GetString(It.IsAny<string>(), It.IsAny<string>())).Returns(ANY_RESOURCE_STRING);

                ProfanityService = new ProfanityService(CacheMock.Object, BadWordsRepository, ResourcesServiceMock.Object);
            }
        }

        [Fact]
        public void ProfanityServiceTests_CheckProfanity_Checks()
        {
            // Setup
            var setup = new TestSetup();

            var result = setup.ProfanityService.CheckProfanity("orgy");
            Assert.NotNull(result);
            Assert.True(result.HasIssues);

            result = setup.ProfanityService.CheckProfanity(null);
            Assert.NotNull(result);
            Assert.True(result.NoData);
            Assert.True(result.HasIssues);

            result = setup.ProfanityService.CheckProfanity("or" + Convert.ToChar(23) + "gy");
            Assert.NotNull(result);
            Assert.True(result.HasIssues);
            Assert.True(result.HasBadCharacters);

            result = setup.ProfanityService.CheckProfanity("clean");
            Assert.NotNull(result);
            Assert.False(result.HasIssues);
        }

        [Fact]
        public void ProfanityServiceTests_LocalizeResult_Localizes()
        {
            // Setup
            var setup = new TestSetup();

            var result = setup.ProfanityService.CheckProfanity("orgy");
            Assert.NotNull(result);
            Assert.True(result.HasIssues);

            Assert.ThrowsAny<Exception>(() => setup.ProfanityService.LocalizeResult(null, FakeResourceStrings.UsCulture));

            result = setup.ProfanityService.CheckProfanity("or" + Convert.ToChar(23) + "gy");

            var localized = setup.ProfanityService.LocalizeResult(result, FakeResourceStrings.UsCulture);
            Assert.NotNull(localized);

            result = setup.ProfanityService.CheckProfanity("orgy");

            localized = setup.ProfanityService.LocalizeResult(result, FakeResourceStrings.UsCulture);
            Assert.NotNull(localized);
        }

        [Fact]
        public void ProfanityServiceTests_LocalizeResult_BadWordCase()
        {
            // Setup
            var setup = new TestSetup();

            var result = setup.ProfanityService.CheckProfanity("orgy");

            var localized = setup.ProfanityService.LocalizeResult(result, FakeResourceStrings.UsCulture);
            Assert.NotNull(localized);
            Assert.Equal(localized.ErrorMessage, TestSetup.ANY_RESOURCE_STRING + ": orgy");

            // Check one more not very realistic path
            result.HasBadCharacters = true;

            localized = setup.ProfanityService.LocalizeResult(result, FakeResourceStrings.UsCulture);
            Assert.NotNull(localized);
            // Leading space is because of result.HasBadCharacters = true;
            Assert.Equal(localized.ErrorMessage, " " + TestSetup.ANY_RESOURCE_STRING + ": orgy");
        }

        [Fact]
        public void ProfanityServiceTests_ProfanityCheckResult_ObjectLogic()
        {
            // Setup
            var setup = new TestSetup();

            var result = setup.ProfanityService.CheckProfanity("orgy");

            result.NoData = true;

            Assert.Equal(result.DefaultErrorMessage, "No data.");
            result.NoData = false;
            result.HasBadCharacters = false;
            result.ProfanityWord = null;

            Assert.ThrowsAny<Exception>(() => result.DefaultErrorMessage);
        }
    }
}
