using BestFor.Dto;
using BestFor.Dto.Account;
using System;
using System.ComponentModel.DataAnnotations;
using R = BestFor.Resources.BestResourcer;

namespace BestFor.Models
{
    /// <summary>
    /// Model/Dto object for user's manage account page.
    /// </summary>
    /// <remarks>
    /// Data annotations do not quite work yet.
    /// Plus I still have no idea how to do the localized validation without setting the thread locale.
    /// I do not want to set the thread locale.
    /// BestResourcer is used for localization.
    /// [Required] ErrorMessageResourceName for property XYZ iz set to AnnotationErrorMessageRequiredXYZ
    /// [EmailAddress] ErrorMessageResourceName for ANY property is set to AnnotationValidationMessageEmailAddress
    /// [Display] Name for property XYZ iz set to AnnotationDisplayNameXYZ
    /// [StringLength] ErrorMessageResourceName for property XYZ iz set to AnnotationErrorMessageStringLength<MaxLength>X<MinLenght>XYZ
    /// ResourceType = typeof(BestResourcer)
    /// ErrorMessageResourceType = typeof(BestResourcer)
    /// Every Annotation<something> must exist as a property in BestResourcer
    /// </remarks>
    public class ProfileEditViewModel : CrudMessagesDto, IBasicUserInfo
    {
        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredEmail", ErrorMessageResourceType = typeof(R))]
        [EmailAddress(ErrorMessageResourceName = "AnnotationValidationMessageEmailAddress", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameEmail", ResourceType = typeof(R))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredPassword", ErrorMessageResourceType = typeof(R))]
        [StringLength(100, MinimumLength = 6,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X6Password", ErrorMessageResourceType = typeof(R))]
        [DataType(DataType.Password)]
        [Display(Name = "AnnotationPassword", ResourceType = typeof(R))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredUserName", ErrorMessageResourceType = typeof(R))]
        [StringLength(100, MinimumLength = 4,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X4UserName", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameUserName", ResourceType = typeof(R))]
        public string UserName { get; set; }

        [StringLength(100, MinimumLength = 3,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X3DisplayName", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameDisplayName", ResourceType = typeof(R))]
        public string DisplayName { get; set; }

        [Display(Name = "AnnotationDisplayNameNumberOfAnswers", ResourceType = typeof(R))]
        public int NumberOfAnswers { get; set; }

        [Display(Name = "AnnotationDisplayNameNumberOfDescriptions", ResourceType = typeof(R))]
        public int NumberOfDescriptions { get; set; }

        [Display(Name = "AnnotationDisplayNameNumberOfVotes", ResourceType = typeof(R))]
        public int NumberOfVotes { get; set; }

        [Display(Name = "AnnotationDisplayNameNumberOfFlags", ResourceType = typeof(R))]
        public int NumberOfFlags { get; set; }

        [Display(Name = "AnnotationDisplayNameNumberOfComments", ResourceType = typeof(R))]
        public int NumberOfComments { get; set; }

        public int Level { get; set; }

        [Display(Name = "AnnotationDisplayNameJoinDate", ResourceType = typeof(R))]
        public DateTime JoinDate { get; set; }

        public string UserImageUrl { get; set; }

        public string UserImageUrlSmall { get; set; }

        [Phone(ErrorMessageResourceName = "AnnotationValidationMessagePhoneNumber", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNamePhoneNumber", ResourceType = typeof(R))]
        public string PhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 4,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X4CompanyName", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameCompanyName", ResourceType = typeof(R))]
        public string CompanyName { get; set; }

        [Url(ErrorMessageResourceName = "AnnotationValidationMessageUrl", ErrorMessageResourceType = typeof(R))]
        [StringLength(100, MinimumLength = 4,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X4WebSite", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameWebSite", ResourceType = typeof(R))]
        public string WebSite { get; set; }

        [StringLength(1000, MinimumLength = 20,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength1000X20UserDescription", ErrorMessageResourceType = typeof(R))]
        [Display(Name = "AnnotationDisplayNameUserDescription", ResourceType = typeof(R))]
        public string UserDescription { get; set; }

        public bool NotFound { get; set; }

        public bool ShowEmail { get; set; }

        public bool ShowPhoneNumber { get; set; }

        public bool ShowCompanyName { get; set; }

        public bool ShowWebSite { get; set; }

        public bool ShowUserDescription { get; set; }

        public bool ShowAvatar { get; set; }

        [Display(Name = "AnnotationDisplayNameDisplayName", ResourceType = typeof(R))]
        public string VisibleDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrWhiteSpace(DisplayName))
                    return UserName;
                return DisplayName;
            }
        }

    }
}
