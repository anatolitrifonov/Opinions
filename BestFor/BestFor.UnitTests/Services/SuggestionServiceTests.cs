using Autofac;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using BestFor.Services.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BestFor.UnitTests.Testables;

namespace BestFor.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    public class SuggestionServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public SuggestionService SuggestionService;
            public FakeSuggestions FakeSuggestions;
            public Mock<ICacheManager> CacheMock;
            public Repository<Suggestion> Repository;
            public TestLoggerFactory TestLoggerFactory;

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                Repository = new Repository<Suggestion>(dataContext);
                CacheMock = new TestCacheManager().CacheMock;

                TestLoggerFactory = new TestLoggerFactory();

                SuggestionService = new SuggestionService(CacheMock.Object, Repository, TestLoggerFactory);
                FakeSuggestions = dataContext.EntitySet<Suggestion>() as FakeSuggestions;
            }
        }

        [Fact]
        public void SuggestionServiceTests_AddSuggestions_AddsSuggestion()
        {
            // Setup
            var setup = new TestSetup();
            // Object we will be adding
            var suggestionDto = new SuggestionDto() { Phrase = "Hello" };

            // Call the method we are testing
            var result = setup.SuggestionService.AddSuggestion(suggestionDto);

            // Check that same Phrase is returned
            Assert.Equal(result.Phrase, suggestionDto.Phrase);

            // Verify cache get was called only once
            setup.CacheMock.Verify(x => x.Get(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA), Times.Once());
            // Verify cache add to cache was called only once
            setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA, It.IsAny<KeyDataSource<Suggestion>>()), Times.Once());
            // Verify repository has the item
            Assert.NotNull(setup.Repository.Queryable().Where(x => x.Phrase == suggestionDto.Phrase).FirstOrDefault());

            var suggestions = setup.SuggestionService.FindSuggestions("Hello");
            Assert.Equal(suggestions.Count(), 1);

            // Lets see if this verifies the code that returns the same suggestion without assert
            var suggestion = setup.SuggestionService.AddSuggestion(suggestionDto);
        }

        [Fact]
        public void SuggestionServiceTests_FindSuggestions_SomeResults()
        {
            // Setup
            var setup = new TestSetup();

            // Call the method we are testing
            var result = setup.SuggestionService.FindSuggestions("test");

            // Check number of test suggestions
            Assert.Equal(result.Count(), setup.FakeSuggestions.NumberOfTestSuggestions);

            // Verify that get was called only once
            setup.CacheMock.Verify(x => x.Get(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA), Times.Once());
            // Verify that Add to cache was called only once
            setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA, It.IsAny<KeyDataSource<Suggestion>>()), Times.Once());

            // Call the method we are testing
            result = setup.SuggestionService.FindSuggestions("abc");

            // Check number of abc suggestions returned is max allowed
            Assert.Equal(result.Count(), KeyDataSource<Suggestion>.DEFAULT_TOP_COUNT);

            // Verify that get was called twice
            setup.CacheMock.Verify(x => x.Get(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA), Times.Exactly(2));
            setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA, It.IsAny<KeyDataSource<Suggestion>>()), Times.Exactly(1));
        }

        [Fact]
        public void SuggestionServiceTests_FindSuggestions_NoResults()
        {
            // Setup
            var setup = new TestSetup();

            // Call the method we are testing
            var result = setup.SuggestionService.FindSuggestions("ztest");

            // Check number of test suggestions
            Assert.Equal(result.Count(), 0);

            // Verify that get was called only once
            setup.CacheMock.Verify(x => x.Get(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA), Times.Once());
            // Verify that Add to cache was called only once
            setup.CacheMock.Verify(x => x.Add(CacheConstants.CACHE_KEY_SUGGESTIONS_DATA, It.IsAny<KeyDataSource<Suggestion>>()), Times.Once());
        }
    }
}
