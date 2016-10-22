using BestFor.Dto;
using BestFor.Services;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace BestFor.UnitTests.Services
{
    [ExcludeFromCodeCoverage]
    public class LinkingHelperTests
    {
        [Fact]
        public void LinkingHelperTests_ConvertAnswerToUrl_Converts()
        {
            var answerDto = new AnswerDto()
            {
                LeftWord = "a",
                RightWord = "b",
                Phrase = "c"
            };

            var result = LinkingHelper.ConvertAnswerToUrl(answerDto);
            Assert.Equal(result, "/best-a-for-b-is-c");

            result = LinkingHelper.ConvertAnswerToText(answerDto);

            Assert.Equal(result, "Best a for b is c");

            var commonStrings = new CommonStringsDto() { Best = "x", For = "y", Is = "z" };

            result = LinkingHelper.ConvertAnswerToText(commonStrings, answerDto);
            Assert.Equal(result, "x a y b z c");

            result = LinkingHelper.ConvertAnswerToUrl(commonStrings, answerDto);
            Assert.Equal(result, "/x-a-y-b-z-c");

            result = LinkingHelper.ConvertAnswerToUrlWithCulture("f", commonStrings, answerDto);
            Assert.Equal(result, "/f/x-a-y-b-z-c");
        }

        [Fact]
        public void LinkingHelperTests_ParseUrlToAnswer_Parses()
        {
            var answerDto = new AnswerDto()
            {
                LeftWord = "a",
                RightWord = "b",
                Phrase = "c"
            };

            var url = "/best-a-for-b-is-c";

            var result = LinkingHelper.ParseUrlToAnswer(url);
            Assert.Equal(result.LeftWord, answerDto.LeftWord);
            Assert.Equal(result.RightWord, answerDto.RightWord);
            Assert.Equal(result.Phrase, answerDto.Phrase);

            var commonStrings = new CommonStringsDto() { Best = "x", For = "y", Is = "z" };
            url = "/x-a-y-b-z-c";

            result = result = LinkingHelper.ParseUrlToAnswer(commonStrings, url);
            Assert.Equal(result.LeftWord, answerDto.LeftWord);
            Assert.Equal(result.RightWord, answerDto.RightWord);
            Assert.Equal(result.Phrase, answerDto.Phrase);
        }

        [Fact]
        public void LinkingHelperTests_Nulls_ReturnsNull()
        {
            var answerDto = new AnswerDto()
            {
                LeftWord = "a",
                RightWord = "b",
                Phrase = "c"
            };

            var result = LinkingHelper.ParseUrlToAnswer(null);
            Assert.Null(result);

            result = LinkingHelper.ParseUrlToAnswer("    ");
            Assert.Null(result);

            result = LinkingHelper.ParseUrlToAnswer("random");
            Assert.Null(result);

            result = LinkingHelper.ParseUrlToAnswer(null, null);
            Assert.Null(result);

            result = LinkingHelper.ParseUrlToAnswer(null, "    ");
            Assert.Null(result);

            var commonStrings = new CommonStringsDto() { Best = "x", For = "y", Is = "z" };
            result = LinkingHelper.ParseUrlToAnswer(commonStrings, "random");
            Assert.Null(result);
        }
    }
}
