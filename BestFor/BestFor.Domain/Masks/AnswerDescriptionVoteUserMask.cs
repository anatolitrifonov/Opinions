using BestFor.Domain.Interfaces;
using BestFor.Dto;

namespace BestFor.Domain.Entities
{
    /// <summary>
    /// Mask answer description vote by user id - index by user id.
    /// 
    /// This class masks answer description vote object giving a different implementation 
    /// of IFirstIndex, ISecondIndex, IDtoConvertable<AnswerDescriptionDto> interfaces.
    /// 
    /// This is done to be able to store different answer description indexes.
    /// 
    /// Not stored in the database because this is an effective copy of the answer description.
    /// 
    /// If any of the properties is of type reference we will be screwed in memory leaks.
    /// </summary>
    public class AnswerDescriptionVoteUserMask : EntityBase, IFirstIndex, ISecondIndex, IDtoConvertable<AnswerDescriptionVoteDto>, IIdIndex
    {
        #region IIdIndex implementation
        /// <summary>
        /// Identity ...
        /// </summary>
        public override int Id { get; set; }
        #endregion

        /// <summary>
        /// Foreign key to answer description
        /// </summary>
        public int AnswerDescriptionId { get; set; }

        /// <summary>
        /// Foreign key to user. We do not have to have it required since users can add answers without
        /// being logged in or registered.
        /// </summary>
        public string UserId { get; set; }


        #region IFirstIndex implementation
        /// <summary>
        /// This is an alternative implementation of index key
        /// </summary>
        public string IndexKey { get { return UserId; } }
        #endregion

        #region ISecondIndex implementation
        public string SecondIndexKey { get { return Id.ToString(); } }

        public int NumberOfEntries { get { return 1; } set { return; } }
        #endregion

        #region IDtoConvertable implementation
        public AnswerDescriptionVoteDto ToDto()
        {
            return new AnswerDescriptionVoteDto()
            {
                UserId = UserId,
                AnswerDescriptionId = AnswerDescriptionId,
                Id = Id,
                DateAdded = DateAdded
            };
        }

        public int FromDto(AnswerDescriptionVoteDto dto)
        {
            Id = dto.Id;
            UserId = dto.UserId;
            AnswerDescriptionId = dto.AnswerDescriptionId;
            DateAdded = dto.DateAdded;

            return Id;
        }
        #endregion
    }
}
