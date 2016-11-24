using BestFor.Dto;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Interface to work with flags for answers and flags for answer descriptions
    /// </summary>
    public interface IFlagService
    {
        DataOperationResult FlagAnswer(AnswerFlagDto answerFlag);

        DataOperationResult FlagAnswerDescription(AnswerDescriptionFlagDto answerFlag);

        /// <summary>
        /// Count flags for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int CountByUserId(string userId);
    }
}
