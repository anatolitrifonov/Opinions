using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BestFor.Dto.Account;
using BestFor.Domain.Interfaces;

namespace BestFor.Domain.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class

    public class ApplicationUser : IdentityUser, IDtoConvertable<ApplicationUserDto>
    {
        /// <summary>
        /// Optional display name. Username will be displayed when blank.
        /// </summary>
        [StringLength(100, ErrorMessage = "*", MinimumLength = 6)] // The {0} must be at least {2} characters long.
        public string DisplayName { get; set; }

        public int NumberOfAnswers { get; set; }

        public int NumberOfDescriptions { get; set; }

        public int NumberOfVotes { get; set; }

        public int NumberOfFlags { get; set; }

        public int NumberOfComments { get; set; }

        /// <summary>
        /// User's favorite opinions category
        /// </summary>
        public string FavoriteCategory { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdated { get; set; }

        public DateTime DateCancelled { get; set; }

        public bool IsCancelled { get; set; }

        [StringLength(1000, MinimumLength = 3)]
        public string CancellationReason { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string CompanyName { get; set; }

        [StringLength(200, MinimumLength = 3)]
        public string WebSite { get; set; }

        /// <summary>
        /// Personal or company description
        /// </summary>
        [StringLength(200, MinimumLength = 3)]
        public string UserDescription { get; set; }

        public bool ShowEmail { get; set; }

        public bool ShowPhoneNumber { get; set; }

        public bool ShowCompanyName { get; set; }

        public bool ShowWebSite { get; set; }

        public bool ShowUserDescription { get; set; }

        public bool ShowAvatar { get; set; }

        /// <summary>
        /// User's level 1 to N
        /// </summary>
        public int Level { get; set; }

        #region IDtoConvertable implementation
        public ApplicationUserDto ToDto()
        {
            return new ApplicationUserDto()
            {
                UserId = Id,
                UserName = UserName,
                NumberOfAnswers = NumberOfAnswers,
                DisplayName = DisplayName,
                Level = Level,

                DateAdded = DateAdded,
                PhoneNumber = PhoneNumber,
                CompanyName = CompanyName,
                WebSite = WebSite,
                UserDescription = UserDescription,
                ShowEmail = ShowEmail,
                ShowPhoneNumber = ShowPhoneNumber,
                ShowCompanyName = ShowCompanyName,
                ShowWebSite = ShowWebSite,
                ShowUserDescription = ShowUserDescription,
                ShowAvatar = ShowAvatar
            };
        }

        public int FromDto(ApplicationUserDto dto)
        {
            Id = dto.UserId;
            UserName = dto.UserName;
            NumberOfAnswers = dto.NumberOfAnswers;
            Level = dto.Level;

            return 1;
        }
        #endregion

    }
}
