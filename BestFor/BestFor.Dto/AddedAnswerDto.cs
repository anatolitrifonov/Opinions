namespace BestFor.Dto
{
    public class AddedAnswerDto : CrudMessagesDto
    {
        public AnswerDto Answer { get; set; }

        public bool IsNew { get; set; }

        public UserLevelingResultDto UserLevelingResult { get; set; }
    }
}
