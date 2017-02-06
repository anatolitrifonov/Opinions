using BestFor.Domain;
using BestFor.Domain.Entities;
using BestFor.Domain.Interfaces;
using System;
using BestFor.Dto;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace BestFor.UnitTests.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class SearchEntryTests
    {
        SearchEntry _searchEntry;

        public SearchEntryTests()
        {
            _searchEntry = new SearchEntry()
            {
                Id = 1,
                DateAdded = new DateTime(2016, 1, 1),
                ObjectState = ObjectState.Unchanged,
                SearchPhrase = "panty",
                UserId = "a"
            };
        }

        [Fact]
        public void SearchEntryTests_IDtoConvertable_Implements()
        {
            SearchEntryDto dto = _searchEntry.ToDto();

            Assert.Equal(dto.Id, _searchEntry.Id);
            Assert.Equal(dto.SearchPhrase, _searchEntry.SearchPhrase);
            Assert.Equal(dto.UserId, _searchEntry.UserId);

            SearchEntry entry = new SearchEntry();
            entry.FromDto(dto);

            Assert.Equal(dto.SearchPhrase, entry.SearchPhrase);
            Assert.Equal(dto.UserId, entry.UserId);
            Assert.Equal(dto.Id, _searchEntry.Id);
        }
    }
}
