using BestFor.Dto;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    public interface IVoteService
    {
        int VoteAnswer(AnswerVoteDto answerVote);

        int VoteAnswerDescription(AnswerDescriptionVoteDto answerVote);

        int CountAnswerVotes(int answerId);

        /// <summary>
        /// Count votes for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int CountByUserId(string userId);
    }
}
