using BestFor.Domain.Interfaces;
using BestFor.Dto;

namespace BestFor.Domain.Entities
{
    /// <summary>
    /// Mask answer vote by user id - index by user id.
    /// 
    /// This class masks answer  vote object giving a different implementation 
    /// of IFirstIndex, ISecondIndex, IDtoConvertable<AnswerVoteDto> interfaces.
    /// 
    /// This is done to be able to store different answer description indexes.
    /// 
    /// Not stored in the database because this is an effective copy of the answer description.
    /// 
    /// If any of the properties is of type reference we will be screwed in memory leaks.
    /// </summary>
    public class AnswerVoteUserMask : EntityBase, IFirstIndex, ISecondIndex, IDtoConvertable<AnswerVoteDto>, IIdIndex
    {
        /// <summary>
        /// Copy data from answer description
        /// </summary>
        /// <param name="answer"></param>
        public AnswerVoteUserMask(AnswerVote answerVote)
        {
            Id = answerVote.Id;
            AnswerId = answerVote.AnswerId;
            UserId = answerVote.UserId;
        }

        #region IIdIndex implementation
        /// <summary>
        /// Identity ...
        /// </summary>
        public override int Id { get; set; }
        #endregion

        /// <summary>
        /// Foreign key to answer
        /// </summary>
        public int AnswerId { get; set; }

        /// <summary>
        /// Foreign key to user. We do not have to have it required since users can add answers without
        /// being logged in or registered.
        /// </summary>
        public string UserId { get; set; }

        #region IFirstIndex implementation
        public string IndexKey { get { return UserId; } }
        #endregion

        #region ISecondIndex implementation
        public string SecondIndexKey { get { return Id.ToString(); } }

        public int NumberOfEntries { get { return 1; } set { return; } }
        #endregion

        #region IDtoConvertable implementation
        public AnswerVoteDto ToDto()
        {
            return new AnswerVoteDto()
            {
                AnswerId = AnswerId,
                UserId = UserId,
                Id = Id,
                DateAdded = DateAdded
            };
        }

        public int FromDto(AnswerVoteDto dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;
            AnswerId = dto.AnswerId;
            // Intentionall do not do that because UI does not set date.
            // We never update votes.
            // Whenever new object is created and inserted we set the creation date on object instantiation
            // Once created and saved this value is only relevant for reading.
            // DateAdded = dto.DateAdded;

            return Id;
        }
        #endregion
    }
}
