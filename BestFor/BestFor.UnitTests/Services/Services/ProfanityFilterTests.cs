using Autofac;
using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Services.Profanity;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Services.Services
{
    /// <summary>
    /// Unit tests for DefaultSuggestions object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ProfanityFilterTests : BaseTest
    {
        [Fact]
        public void ProfanityFilter_Character17_NotAllowed()
        {
            Assert.True(!ProfanityFilter.AllCharactersAllowed("\"(" + new string(new char[] { (char)17 }) + ")[\\|/"));
        }

        [Fact]
        public void ProfanityFilter_GraveAccent_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("`"));
        }

        [Fact]
        public void ProfanityFilter_Exclamation_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("\r\n!"));
        }

        [Fact]
        public void ProfanityFilter_Space_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed(" \r\n "));
        }

        [Fact]
        public void ProfanityFilter_DoubleQuote_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("\"\r\n\""));
        }

        [Fact]
        public void ProfanityFilter_Tilda_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("~\r\n~"));
        }

        [Fact]
        public void ProfanityFilter_AtSymbol_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("@\r\n@"));
        }

        [Fact]
        public void ProfanityFilter_NumberSign_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("#\r\n#"));
        }

        [Fact]
        public void ProfanityFilter_DollarSign_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("$\r\n$"));
        }

        [Fact]
        public void ProfanityFilter_PercentSign_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("%\r\n%"));
        }

        [Fact]
        public void ProfanityFilter_CircumflexAccent_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("^\r\n^"));
        }

        [Fact]
        public void ProfanityFilter_Asterisk_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("*\r\n*a*"));
        }

        [Fact]
        public void ProfanityFilter_Ampersand_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("&\r\n&a*"));
        }

        [Fact]
        public void ProfanityFilter_A_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("A"));
        }

        [Fact]
        public void ProfanityFilter_LeftParenthesis_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("(\r\n("));
        }

        [Fact]
        public void ProfanityFilter_RightParenthesis_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed(")\r\n)"));
        }

        [Fact]
        public void ProfanityFilter_Minus_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("-\r\n-"));
        }

        [Fact]
        public void ProfanityFilter_Underscore_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("_\r\n_"));
        }

        [Fact]
        public void ProfanityFilter_LessThan_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("<\r\n<"));
        }

        [Fact]
        public void ProfanityFilter_QuestionMark_Allowed()
        {
            Assert.True(ProfanityFilter.AllCharactersAllowed("?\r\n?"));
        }

        [Fact]
        public void ProfanityFilter_All_Allowed()
        {
            string data = "` ~ ! @ # $ % ^ & * ()- _+\"={}[];:'/ < > .` \\ | ~ ! @ # $ % ^ & * ( ) - _ + = { [ } ] ; : ' \" < , > . ? /";
            string tomatch = data + "\r\na\r\na" + data;
            var result = ProfanityFilter.AllCharactersAllowed(tomatch);
            Assert.True(result);
        }

        [Fact]
        public void ProfanityCleanup_VariationOfA_Replaced()
        {
            var result = ProfanityFilter.CleanupData("A a @ b");
            Assert.True(result == "a a @ b");
        }

        [Fact]
        public void ProfanityCleanup_VariationOfB_Replaced()
        {
            var result = ProfanityFilter.CleanupData("I3");
            Assert.True(result == "i3");
        }

        [Fact]
        public void ProfanityCheck_I3_Replaced()
        {
            var result = ProfanityFilter.CleanupData("I3");
            Assert.True(result == "i3");
        }

        [Fact]
        public void ProfanityCheck_Panty_Detected()
        {
            var repo = new Repository<BadWord>(resolver.Resolve<IDataContext>());

            var badWord = "panty";
            var checkPhrase = "She was whereing panty";
            var result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.True(result == badWord);
        }

        [Fact]
        public void ProfanityCheck_FirstDisallowedCharacter_Returns()
        {
            char bad = Convert.ToChar(23);
            char random = Convert.ToChar(17);
            var result = ProfanityFilter.FirstDisallowedCharacter("I3" + bad + random);
            Assert.Equal(result, bad.ToString());

            result = ProfanityFilter.FirstDisallowedCharacter("I3");
            Assert.Null(result);
        }

        [Fact]
        public void ProfanityCheck_CleanupData_Corners()
        {
            var result = ProfanityFilter.CleanupData(null);
            Assert.Null(result);
            result = ProfanityFilter.CleanupData("    ");
            Assert.Equal(result, "    ");
        }

        [Fact]
        public void ProfanityCheck_CheckContains_Additional()
        {
            var result = ProfanityFilter.CheckContains("a", "bad", " !~");
            Assert.Null(result);

            result = ProfanityFilter.CheckContains("my !bad!", "bad", " !~");
            Assert.Equal(result, "!bad!");
        }

        [Fact]
        public void ProfanityCheck_GetProfanity_Extensive()
        {
            var repo = new Repository<BadWord>(resolver.Resolve<IDataContext>());

            var result = ProfanityFilter.GetProfanity(null, null);
            Assert.Null(result);
            var badWord = "panty";
            var checkPhrase = "She was whereing panty.";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);

            checkPhrase = "panty well";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);

            checkPhrase = " panty well";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);

            checkPhrase = "panty";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);

            checkPhrase = "panty.";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);

            checkPhrase = "$panty$";
            result = ProfanityFilter.GetProfanity(checkPhrase, repo.List());
            Assert.Equal(result, badWord);
        }
    }
}
