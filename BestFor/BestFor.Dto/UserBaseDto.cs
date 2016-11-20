using BestFor.Dto.Account;

namespace BestFor.Dto
{
    /// <summary>
    /// Used to ensure that DtoConvertable interface returns dto objects from this library
    /// </summary>
    public abstract class UserBaseDto : BaseDto
    {
        public string UserId { get; set; }

        private ApplicationUserDto _user;
        /// <summary>
        /// User that added this object
        /// </summary>
        /// <remarks>Only create when property is accessed.</remarks>
        public ApplicationUserDto User
        {
            get
            {
                if (_user == null)
                    _user = new ApplicationUserDto();
                return _user;
            }
            set
            {
                _user = value;
            }
        }
    }
}
