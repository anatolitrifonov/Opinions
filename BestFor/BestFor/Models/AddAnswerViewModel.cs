using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using R = BestFor.Resources.BestResourcer;

namespace BestFor.Models
{
    /// <summary>
    /// Model/Dto object for adding the new answer from home page
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
    public class AddAnswerViewModel
    {
        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredLeftWord", ErrorMessageResourceType = typeof(R))]
        [StringLength(200, MinimumLength = 3,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength200X3LeftWord", ErrorMessageResourceType = typeof(R))]
        //Do not need display name since we know view is rendering it
        //[Display(Name = "AnnotationDisplayNameLeftWord", ResourceType = typeof(R))]
        public string LeftWord { get; set; }

        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredRightWord", ErrorMessageResourceType = typeof(R))]
        [StringLength(200, MinimumLength = 3,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength200X3RightWord", ErrorMessageResourceType = typeof(R))]
        //Do not need display name since we know view is rendering it
        //[Display(Name = "AnnotationDisplayNameRightWord", ResourceType = typeof(R))]
        public string RightWord { get; set; }

        [Required(ErrorMessageResourceName = "AnnotationErrorMessageRequiredPhrase", ErrorMessageResourceType = typeof(R))]
        [StringLength(200, MinimumLength = 3,
            ErrorMessageResourceName = "AnnotationErrorMessageStringLength200X3Phrase", ErrorMessageResourceType = typeof(R))]
        //[Display(Name = "AnnotationDisplayNamePhrase", ResourceType = typeof(R))]
        public string Phrase { get; set; }
    }
}
