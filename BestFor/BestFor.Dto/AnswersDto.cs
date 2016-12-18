using System.Collections.Generic;

namespace BestFor.Dto
{
    public class AnswersDto : CrudMessagesDto
    {
        public List<AnswerDto> Answers { get; set; }

        /// <summary>
        /// When true this is search result on the left words
        /// When false this is search result on the right word
        /// </summary>
        public bool IsLeft { get; set; } = true;

        public string SearchKeyword { get; set; }
    }
}
