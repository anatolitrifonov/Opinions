using BestFor.Domain.Entities;
using BestFor.Domain.Interfaces;
using BestFor.Dto;

namespace BestFor.Domain.Masks
{
    /// <summary>
    /// Mask answer description by user id - index by user id.
    /// 
    /// This class masks answer description object giving a different implementation 
    /// of IFirstIndex, ISecondIndex, IDtoConvertable<AnswerDescriptionDto> interfaces.
    /// 
    /// This is done to be able to store different answer description indexes.
    /// 
    /// Not stored in the database because this is an effective copy of the answer description.
    /// 
    /// If any of the properties is of type reference we will be screwed in memory leaks.
    /// </summary>
    public class AnswerDescriptionUserMask : IFirstIndex, ISecondIndex, IDtoConvertable<AnswerDescriptionDto>, IIdIndex
    {
        /// <summary>
        /// Copy data from answer description
        /// </summary>
        /// <param name="answer"></param>
        public AnswerDescriptionUserMask(AnswerDescription answerDescription)
        {
            Id = answerDescription.Id;
            Description = answerDescription.Description;
            UserId = answerDescription.UserId;
        }

        #region IIdIndex implementaion
        public int Id { get; set; }
        #endregion

        public int AnswerId { get; set; }

        public string Description { get; set; }

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
        public AnswerDescriptionDto ToDto()
        {
            return new AnswerDescriptionDto()
            {
                Description = Description,
                AnswerId = AnswerId,
                Id = Id,
                UserId = UserId
            };
        }

        public int FromDto(AnswerDescriptionDto dto)
        {
            Description = dto.Description;
            AnswerId = dto.AnswerId;
            Id = dto.Id;
            UserId = dto.UserId;

            return Id;
        }
        #endregion

    }
}
