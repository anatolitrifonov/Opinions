using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestFor.Domain.Entities
{
    public class UserAchievement : EntityBase
    {
        public string UserId { get; set;  }

        public string AchievementId { get; set; }

        /// <summary>
        /// Parent user object. User that added the answer.
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        /// <summary>
        /// Parent user object. User that added the answer.
        /// </summary>
        [ForeignKey("AchievementId")]
        public Achievement Achievement { get; set; }
    }
}
