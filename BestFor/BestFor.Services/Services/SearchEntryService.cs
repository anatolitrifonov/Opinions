using System.Linq;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    public class SearchEntryService : ISearchEntryService
    {
        private const int DEFAULT_TOP_COUNT = 50;

        private readonly IRepository<SearchEntry> _searchEntryRepository;
        private readonly ILogger _logger;

        public SearchEntryService(
            IRepository<SearchEntry> searchEntryRepository,
            ILoggerFactory loggerFactory)
        {
            _searchEntryRepository = searchEntryRepository;
            _logger = loggerFactory.CreateLogger<SearchEntryService>();
        }

        #region ISearchEntryService implementation
        /// <summary>
        /// Return top x most recent searches.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SearchEntryDto> FindRecentSearchEntries()
        {
            return _searchEntryRepository.List().OrderByDescending(x => x.DateAdded)
                .Take(DEFAULT_TOP_COUNT).Select(y => y.ToDto());
        }

        public SearchEntryDto AddSearchEntry(SearchEntryDto searchEntryDto)
        {
            var searchEntry = new SearchEntry();
            searchEntry.FromDto(searchEntryDto);

            _searchEntryRepository.Insert(searchEntry);
            var task = _searchEntryRepository.SaveChangesAsync();
            task.Wait();

            searchEntryDto.DateAdded = searchEntry.DateAdded;
            searchEntryDto.Id = searchEntry.Id;

            return searchEntryDto;
        }
        #endregion
    }
}
