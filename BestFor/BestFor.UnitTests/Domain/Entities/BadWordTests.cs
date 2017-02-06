using BestFor.Domain;
using BestFor.Domain.Entities;
using BestFor.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace BestFor.UnitTests.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class BadWordTests
    {
        BadWord _badWord;

        public BadWordTests()
        {
            _badWord = new BadWord()
            {
                Id = 1,
                DateAdded = new DateTime(2016, 1, 1),
                ObjectState = ObjectState.Unchanged,
                Phrase = "panty"
            };
        }

        [Fact]
        public void BadWordTests_IFirstIndex_Implements()
        {
            var firstIndex = _badWord as IFirstIndex;

            // Verify first index is implemented correctly
            Assert.Equal(firstIndex.IndexKey, _badWord.Phrase.ToString());
            Assert.NotEqual(firstIndex.IndexKey, _badWord.DateAdded.ToString());
        }
    }
}
