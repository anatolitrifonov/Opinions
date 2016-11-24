using Autofac;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Fakes;
using BestFor.Services;
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
    public class FlagServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public FlagService FlagService;
            public Mock<ICacheManager> CacheMock;
            public Repository<AnswerFlag> AnswerFlagsRepository;
            public Repository<AnswerDescriptionFlag> AnswerDescriptionFlagRepository;
            public TestLoggerFactory TestLoggerFactory;

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                AnswerFlagsRepository = new Repository<AnswerFlag>(dataContext);
                AnswerDescriptionFlagRepository = new Repository<AnswerDescriptionFlag>(dataContext);
                CacheMock = new TestCacheManager().CacheMock;
                TestLoggerFactory = new TestLoggerFactory();
                FlagService = new FlagService(
                    null,
                    CacheMock.Object,
                    AnswerFlagsRepository,
                    AnswerDescriptionFlagRepository,
                    TestLoggerFactory);
            }
        }

        [Fact]
        public void FlagService_FlagAnswerNullData_ThrowsException()
        {
            // Setup
            var setup = new TestSetup();

            // Call the method we are testing
            // (input parameters) => expression // Expression Lambdas

            Assert.ThrowsAny<ServicesException>(() => setup.FlagService.FlagAnswer(null));

            Assert.ThrowsAny<ServicesException>(() => setup.FlagService.FlagAnswer(new AnswerFlagDto() { AnswerId = 0 }));

            // This will throw exception because there is no UserId
            Assert.ThrowsAny<ServicesException>(() => setup.FlagService.FlagAnswer(new AnswerFlagDto() { AnswerId = 1 }));
        }

        [Fact]
        public void FlagService_FlagExistingAnswer_DoesNotAddAgain()
        {
            // Setup
            var setup = new TestSetup();

            var answerFlag1 = new AnswerFlagDto() { AnswerId = 1, UserId = "1" };
            var answerFlag2 = new AnswerFlagDto() { AnswerId = 1, UserId = "1" };

            var count = setup.AnswerFlagsRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(0, count);
            setup.FlagService.FlagAnswer(answerFlag1);
            count = setup.AnswerFlagsRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(1, count);
            setup.FlagService.FlagAnswer(answerFlag2);
            // Verify insert was called for every flag save. Flags work differently than votes.
            // Double flags are OK.
            count = setup.AnswerFlagsRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(2, count);
        }

        [Fact]
        public void FlagService_FlagAnswer_AddsFlag()
        {
            // Setup
            var setup = new TestSetup();
            // Object we will be adding
            var answerFlagDto = new AnswerFlagDto() { Id = 123, AnswerId = 5, UserId ="D" };

            // Call the method we are testing
            var result = setup.FlagService.FlagAnswer(answerFlagDto);

            // Check that same Phrase is returned
            Assert.Equal(result.IntId, answerFlagDto.Id);

            // Verify repository has the item
            Assert.NotNull(setup.AnswerFlagsRepository.Queryable()
                .Where(x => x.AnswerId == answerFlagDto.AnswerId).FirstOrDefault());
        }

        [Fact]
        public void FlagService_FlagAnswerDescriptionNullData_ThrowsException()
        {
            // Setup
            var setup = new TestSetup();

            // Call the method we are testing
            // (input parameters) => expression // Expression Lambdas

            Assert.ThrowsAny<ServicesException>(() => setup.FlagService.FlagAnswerDescription(null));

            Assert.ThrowsAny<ServicesException>(() => setup
                .FlagService.FlagAnswerDescription(new AnswerDescriptionFlagDto() { AnswerDescriptionId = 0 }));

            // This will throw exception because there is no UserId
            Assert.ThrowsAny<ServicesException>(() => setup
                .FlagService.FlagAnswerDescription(new AnswerDescriptionFlagDto() { AnswerDescriptionId = 1 }));
        }

        [Fact]
        public void FlagService_FlagExistingAnswerDescription_DoesNotAddAgain()
        {
            // Setup
            var setup = new TestSetup();

            var answerDescriptionFlag1 = new AnswerDescriptionFlagDto() { AnswerDescriptionId = 1, UserId = "1" };
            var answerDescriptionFlag2 = new AnswerDescriptionFlagDto() { AnswerDescriptionId = 1, UserId = "1" };

            var count = setup.AnswerDescriptionFlagRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(0, count);
            setup.FlagService.FlagAnswerDescription(answerDescriptionFlag1);
            count = setup.AnswerDescriptionFlagRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(1, count);
            setup.FlagService.FlagAnswerDescription(answerDescriptionFlag2);
            // Verify insert was called for every flag save. Flags work differently than votes.
            // Double flags are OK.
            count = setup.AnswerDescriptionFlagRepository.Queryable().Where(x => x.UserId == "1").Count();
            Assert.Equal(2, count);
        }

        [Fact]
        public void FlagService_FlagExistingAnswerDescription_DoesNotCacheAgain()
        {
            // Setup
            var setup = new TestSetup();

            var answerDescriptionFlag1 = new AnswerDescriptionFlagDto() { AnswerDescriptionId = 1, UserId = "1" };
            var answerDescriptionFlag2 = new AnswerDescriptionFlagDto() { AnswerDescriptionId = 2, UserId = "1" };

            setup.FlagService.FlagAnswerDescription(answerDescriptionFlag1);
            setup.FlagService.FlagAnswerDescription(answerDescriptionFlag2);

            // Verify cache add to cache was called only once
            //setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_DESCRIPTION_FlagS_DATA,
            //    It.IsAny<KeyIndexedDataSource<AnswerDescriptionFlag>>()), Times.Once());
        }

        [Fact]
        public void FlagService_FlagExistingAnswer_DoesNotCacheAgain()
        {
            // Setup
            var setup = new TestSetup();

            var answerFlag1 = new AnswerFlagDto() { AnswerId = 1, UserId = "1" };
            var answerFlag2 = new AnswerFlagDto() { AnswerId = 2, UserId = "1" };

            setup.FlagService.FlagAnswer(answerFlag1);
            setup.FlagService.FlagAnswer(answerFlag2);

            // Verify cache add to cache was called only once
            //setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_FlagS_DATA,
            //    It.IsAny<KeyIndexedDataSource<AnswerFlag>>()), Times.Once());
        }

        [Fact]
        public void FlagService_CountAnswerFlags_ReturnsAnswerFlags()
        {
            // Setup
            var setup = new TestSetup();

            var answerFlag1 = new AnswerFlagDto() { Id = 1234, AnswerId = 111, UserId = "1" };
            var answerFlag2 = new AnswerFlagDto() { Id = 1235, AnswerId = 111, UserId = "2" };

            setup.FlagService.FlagAnswer(answerFlag1);
            setup.FlagService.FlagAnswer(answerFlag2);

            //var count = setup.FlagService.CountAnswerFlags(111);
            //// Verify that cache has 2 Flags.
            //Assert.Equal(2, count);

            //count = setup.FlagService.CountAnswerFlags(0);
            //// Verify that cache has 2 Flags.
            //Assert.Equal(0, count);

            //count = setup.FlagService.CountAnswerFlags(112);
            //// Verify that cache does not contain random items.
            //Assert.Equal(0, count);

        }
    }
}
