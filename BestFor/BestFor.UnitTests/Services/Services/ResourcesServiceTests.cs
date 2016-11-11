using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.Services;
using BestFor.UnitTests.Testables;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace BestFor.UnitTests.Services.Services
{
    [ExcludeFromCodeCoverage]
    public class ResourcesServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public ResourcesService ResourcesService;
            public Mock<ICacheManager> CacheMock;
            public Repository<ResourceString> ResourceStringsRepository;
            public TestLoggerFactory TestLoggerFactory;

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                ResourceStringsRepository = new Repository<ResourceString>(dataContext);
                CacheMock = new TestCacheManager().CacheMock;
                ResourcesService = new ResourcesService(CacheMock.Object, ResourceStringsRepository);
                TestLoggerFactory = null;

        }
    }

        [Fact]
        public void ResourcesServiceTests_GetCommonStrings_ReturnsCommonStrings()
        {
            // Setup
            var setup = new TestSetup();

            var commonStrings = setup.ResourcesService.GetCommonStrings(null);
            Assert.NotNull(commonStrings);

            commonStrings = setup.ResourcesService.GetCommonStrings(string.Empty);
            Assert.Equal(commonStrings.Best, "Best");

            // one more time to test that it is not loading twice
            commonStrings = setup.ResourcesService.GetCommonStrings(string.Empty);
            Assert.Equal(commonStrings.Best, "Best");

            var globalData = setup.ResourcesService.GetCommonStringsForAllCultures();
            Assert.NotNull(globalData);
        }

        [Fact]
        public void ResourcesServiceTests_GetString_ReturnsString()
        {
            // Setup
            var setup = new TestSetup();
            var testKey = "ac";
            // no culture + no key -> null
            var testString = setup.ResourcesService.GetString(null, null);
            Assert.Null(testString);

            // no key -> null
            testString = setup.ResourcesService.GetString(FakeResourceStrings.UsCulture, null);
            Assert.Null(testString);

            // no culture -> key
            testString = setup.ResourcesService.GetString(null, testKey);
            Assert.Equal(testString, testKey);

            // Find the random string in English
            testString = setup.ResourcesService.GetString(FakeResourceStrings.UsCulture, FakeResourceStrings.RandomStringKey);
            var usResourceString = setup.ResourceStringsRepository.Queryable()
                .First(x => x.CultureName == FakeResourceStrings.UsCulture && x.Key == FakeResourceStrings.RandomStringKey);
            Assert.Equal(testString, usResourceString.Value);

            // Find the random string in Russian
            testString = setup.ResourcesService.GetString(FakeResourceStrings.RuCulture, FakeResourceStrings.RandomStringKey);
            var ruResourceString = setup.ResourceStringsRepository.Queryable()
                .First(x => x.CultureName == FakeResourceStrings.RuCulture && x.Key == FakeResourceStrings.RandomStringKey);
            Assert.Equal(testString, ruResourceString.Value);

            // Find the random string in unknown culture -> us version returned.
            testString = setup.ResourcesService.GetString("test", FakeResourceStrings.RandomStringKey);
            Assert.Equal(testString, usResourceString.Value);

            // Test replace patterns.
            testString = setup.ResourcesService.GetString(FakeResourceStrings.UsCulture, FakeResourceStrings.RandomPatternStringKey);
            Assert.Equal(testString, FakeResourceStrings.RandomPatternStringTestValueUs);

        }

        [Fact]
        public void ResourcesServiceTests_ReplacePattern_AdditionalTests()
        {
            // Setup
            var setup = new TestSetup();

            var testString = setup.ResourcesService.ReplacePatterns(null, null);
            Assert.Null(testString);

            testString = setup.ResourcesService.ReplacePatterns("      ", null);
            Assert.Equal(testString, "      ");
        }

        [Fact]
        public void ResourcesServiceTests_GetStringsAsJson_Returns()
        {
            // Setup
            var setup = new TestSetup();

            var testObject = setup.ResourcesService.GetStringsAsJson(null, null);
            Assert.Null(testObject);

            testObject = setup.ResourcesService.GetStringsAsJson(null, new string[] { FakeResourceStrings.RandomStringKey });
            Assert.NotNull(testObject);
            Assert.Equal(testObject.ToString(), "{\r\n  \"randomString\": \"randomString\"\r\n}");

            testObject = setup.ResourcesService.GetStringsAsJson(FakeResourceStrings.UsCulture, new string[] { FakeResourceStrings.RandomStringKey });
            Assert.NotNull(testObject);
            Assert.Equal(testObject.ToString(), "{\r\n  \"randomString\": \"" + FakeResourceStrings.RandomStringValueUs + "\"\r\n}");

            testObject = setup.ResourcesService.GetStringsAsJson(FakeResourceStrings.RuCulture, new string[] { FakeResourceStrings.RandomStringKey });
            Assert.NotNull(testObject);
            Assert.Equal(testObject.ToString(), "{\r\n  \"randomString\": \"" + FakeResourceStrings.RandomStringValueRu + "\"\r\n}");
        }

        [Fact]
        public void ResourcesServiceTests_GetStringsAsJavaScript_Returns()
        {
            // Setup
            var setup = new TestSetup();

            var testString = setup.ResourcesService.GetStringsAsJavaScript(null, null, null);
            Assert.Null(testString);

            testString = setup.ResourcesService.GetStringsAsJavaScript(null, null, new string[] { FakeResourceStrings.RandomStringKey });
            Assert.NotNull(testString);
            Assert.Equal(testString, "<script>\n\rvar  = {\n\r\"randomString\" : \"randomString\"\n\r}\r\n</script>\n\r");

            testString = setup.ResourcesService.GetStringsAsJavaScript(null, null,
                new string[] { FakeResourceStrings.RandomStringKey, FakeResourceStrings.RandomPatternStringKey, "a", "b" });
            Assert.NotNull(testString);
        }

        [Fact]
        public void ResourcesServiceTests_GetStrings_AdditionalTests()
        {
            // Setup
            var setup = new TestSetup();

            var testStrings = setup.ResourcesService.GetStrings(FakeResourceStrings.UsCulture, 
                new string[] { FakeResourceStrings.RandomStringKey, null, "  " });
            Assert.NotNull(testStrings);
            Assert.Equal(testStrings[0], FakeResourceStrings.RandomStringValueUs);
            Assert.Equal(testStrings[1], null);
            Assert.Equal(testStrings[2], "  ");

            testStrings = setup.ResourcesService.GetStrings(FakeResourceStrings.UsCulture, null);
            Assert.Null(testStrings);
        }

    }
}
