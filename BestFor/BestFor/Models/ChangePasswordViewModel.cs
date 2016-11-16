using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using R = BestFor.Resources.BestResourcer;

namespace BestFor.Models
{
    /// <summary>
    /// Model/Dto object for change password page.
    /// </summary>
    /// <remarks>
    /// BestResourcer is used for localization.
    /// [Required] ErrorMessageResourceName for property XYZ iz set to AnnotationErrorMessageRequiredXYZ
    /// [EmailAddress] ErrorMessageResourceName for ANY property is set to AnnotationValidationMessageEmailAddress
    /// [Display] Name for property XYZ iz set to AnnotationDisplayNameXYZ
    /// [StringLength] ErrorMessageResourceName for property XYZ iz set to AnnotationErrorMessageStringLength<MaxLength>X<MinLenght>XYZ
    /// ResourceType = typeof(BestResourcer)
    /// ErrorMessageResourceType = typeof(BestResourcer)
    /// Every Annotation<something> must exist as a property in BestResourcer
    /// </remarks>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredPassword", ErrorMessageResourceType = typeof(R))]
        [StringLength(100, MinimumLength = 6,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X6Password", ErrorMessageResourceType = typeof(R))]
        [DataType(DataType.Password)]
        [Display(Name = "AnnotationOldPassword", ResourceType = typeof(R))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredPassword", ErrorMessageResourceType = typeof(R))]
        [StringLength(100, MinimumLength = 6,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength100X6Password", ErrorMessageResourceType = typeof(R))]
        [DataType(DataType.Password)]
        [Display(Name = "AnnotationPassword", ResourceType = typeof(R))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "AnnotationConfirmPassword", ResourceType = typeof(R))]
        [Compare("NewPassword", ErrorMessageResourceName = "AnnotationErrorMessageComparePassword", ErrorMessageResourceType = typeof(R))]
        public string ConfirmNewPassword { get; set; }

        public string SuccessMessage { get; set; }
    }
}
