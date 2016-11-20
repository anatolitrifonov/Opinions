namespace BestFor.Dto
{
    /// <summary>
    /// Model dto object used for adding answer description page
    /// </summary>
    public class AnswerDescriptionDto : UserBaseDto
    {
        public AnswerDto Answer { get; set; }

        public int AnswerId { get; set; }

        public string Description { get; set; }
    }
}
