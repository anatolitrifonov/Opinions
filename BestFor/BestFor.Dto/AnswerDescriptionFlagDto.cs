namespace BestFor.Dto
{
    /// <summary>
    /// Represents flag for the answer description
    /// </summary>
    public class AnswerDescriptionFlagDto : UserBaseDto
    {
        public AnswerDescriptionFlagDto()
        {
        }

        public int AnswerDescriptionId { get; set; }

        public string Reason { get; set; }
    }
}
