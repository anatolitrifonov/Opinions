using BestFor.Dto;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    /// <summary>
    /// This service adds search entries and allows to look through them.
    /// </summary>
    public interface ISearchEntryService
    {
        IEnumerable<SearchEntryDto> FindRecentSearchEntries();

        SearchEntryDto AddSearchEntry(SearchEntryDto searchEntry);
    }
}
