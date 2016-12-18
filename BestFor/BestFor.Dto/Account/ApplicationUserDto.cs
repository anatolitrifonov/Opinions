using System;

namespace BestFor.Dto.Account
{
    public class ApplicationUserDto : BaseDto, IBasicUserInfo
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Return user name if blank.
        /// </summary>
        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName) || string.IsNullOrWhiteSpace(_displayName)) return UserName;
                return _displayName;

            }
            set
            {
                _displayName = value;
            }
        }

        public int NumberOfAnswers { get; set; }

        public int NumberOfDescriptions { get; set; }

        public int NumberOfVotes { get; set; }

        public int NumberOfFlags { get; set; }

        public int NumberOfComments { get; set; }

        /// <summary>
        /// User's favorite opinions category
        /// </summary>
        public string FavoriteCategory { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCancelled { get; set; }

        public bool IsCancelled { get; set; }

        public string CancellationReason { get; set; }

        /// <summary>
        /// User's level 1 to N
        /// </summary>
        public int Level { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }

        public string WebSite { get; set; }

        public string UserDescription { get; set; }

        public bool ShowEmail { get; set; }

        public bool ShowPhoneNumber { get; set; }

        public bool ShowCompanyName { get; set; }

        public bool ShowWebSite { get; set; }

        public bool ShowUserDescription { get; set; }

        public bool ShowAvatar { get; set; }

        #region Extra Properties
        /// <summary>
        /// When ApplicationUser class is instantiated, IsStatisticsCached property is false by defaul.
        /// It will be set to true once user's statistics is cached which is usually when it is requested for the first time.
        /// </summary>        
        public bool IsStatisticsCached { get; set; }
        /// <summary>
        /// When this class is instantiated, this property is false.
        /// It will be set to true once user's image url is requested for the first time.
        /// </summary>
        public bool IsImageCached { get; set; }

        public bool IsSignedIn { get; set; }

        public string UserImageUrl { get; set; }

        public string UserImageUrlSmall { get; set; }
        #endregion
    }
}
