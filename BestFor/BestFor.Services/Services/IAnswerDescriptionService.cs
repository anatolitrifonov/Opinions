using BestFor.Dto;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Interface for answer descriptions service. Works with AnswerDescription object.
    /// </summary>
    public interface IAnswerDescriptionService
    {

        /// <summary>
        /// Add AnswerDescription
        /// </summary>
        /// <param name="answerDescription"></param>
        /// <returns></returns>
        DataOperationResult AddAnswerDescription(AnswerDescriptionDto answerDescription);
        
        /// <summary>
        /// Find all descriptions of a given answer
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        IEnumerable<AnswerDescriptionDto> FindByAnswerId(int answerId);

        /// <summary>
        /// Find a given description from its id.
        /// </summary>
        /// <param name="answerDescriptionId"></param>
        /// <returns></returns>
        AnswerDescriptionDto FindByAnswerDescriptionId(int answerDescriptionId);

        /// <summary>
        /// Find all answers descriptions with no user going directly to the database
        /// </summary>
        /// <returns></returns>
        IEnumerable<AnswerDescriptionDto> FindDirectBlank();

        /// <summary>
        /// Find all answer descriptions for a given answer going directly to database
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        IEnumerable<AnswerDescriptionDto> FindDirectByAnswerId(int answerId);

        /// <summary>
        /// Find all answer descriptions for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<AnswerDescriptionDto> FindDirectByUserId(string userId);


        /// <summary>
        /// Count descriptions for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int CountByUserId(string userId);
    }
}
