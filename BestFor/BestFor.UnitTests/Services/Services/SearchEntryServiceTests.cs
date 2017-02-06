using Autofac;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Fakes;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using BestFor.Services.Services;
using BestFor.UnitTests.Testables;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace BestFor.UnitTests.Services.Services
{
    [ExcludeFromCodeCoverage]
    public class SearchEntryServiceTests
    {
        /// <summary>
        /// Constructor sets up the needed data.
        /// </summary>
        private class TestSetup
        {
            public SearchEntryService SearchEntryService;
            public Repository<SearchEntry> Repository;
            public TestLoggerFactory TestLoggerFactory;
            // public FakeSuggestions FakeSuggestions;

            public TestSetup()
            {
                var dataContext = new FakeDataContext();
                Repository = new Repository<SearchEntry>(dataContext);
                TestLoggerFactory = new TestLoggerFactory();
                SearchEntryService = new SearchEntryService(Repository, TestLoggerFactory);
                // FakeSuggestions = dataContext.EntitySet<Suggestion>() as FakeSuggestions;
            }
        }

        [Fact]
        public void SearchEntryServiceTests_AddSearchEntry_AddsSearchEntry()
        {
            // Setup
            var setup = new TestSetup();
            // Object we will be adding
            var searchEntryDto = new SearchEntryDto() { SearchPhrase = "Hello" };

            // Call the method we are testing
            var result = setup.SearchEntryService.AddSearchEntry(searchEntryDto);

            // Check that same Phrase is returned
            Assert.Equal(result.SearchPhrase, searchEntryDto.SearchPhrase);

            // Verify repository has the item
            Assert.NotNull(setup.Repository.Queryable().Where(x => x.SearchPhrase == searchEntryDto.SearchPhrase).FirstOrDefault());

            var searchEntries = setup.SearchEntryService.FindRecentSearchEntries();
            // One from this test and one default.
            Assert.Equal(searchEntries.Count(), 2);
        }
    }
}
