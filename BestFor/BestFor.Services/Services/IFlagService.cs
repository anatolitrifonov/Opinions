using BestFor.Dto;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Interface to work with flags for answers and flags for answer descriptions
    /// </summary>
    public interface IFlagService
    {
        int FlagAnswer(AnswerFlagDto answerFlag);

        int FlagAnswerDescription(AnswerDescriptionFlagDto answerFlag);

        /// <summary>
        /// Count flags for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int CountByUserId(string userId);
    }
}
