namespace BestFor.Dto
{
    /// <summary>
    /// Represents flag for the answer
    /// </summary>
    public class AnswerFlagDto : UserBaseDto
    {
        public AnswerFlagDto()
        {
        }

        public int AnswerId { get; set; }

        public string Reason { get; set; }
    }
}
