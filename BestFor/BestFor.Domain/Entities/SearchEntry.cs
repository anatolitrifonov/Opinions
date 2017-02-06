using BestFor.Domain.Interfaces;
using BestFor.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestFor.Domain.Entities
{
    public class SearchEntry : EntityBase, IDtoConvertable<SearchEntryDto>
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        public string SearchPhrase { get; set; }

        /// <summary>
        /// Foreign key to user. Checking if it has to be marked as [Required]. We do not have to have it 
        /// required since users can add answers without being logged in or registered.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Parent user object. User that added the answer.
        /// </summary>
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        #region IDtoConvertable implementation
        public SearchEntryDto ToDto()
        {
            return new SearchEntryDto()
            {
                SearchPhrase = SearchPhrase,
                UserId = UserId,
                DateAdded = DateAdded,
                Id = Id
            };
        }

        public int FromDto(SearchEntryDto dto)
        {
            SearchPhrase = dto.SearchPhrase;
            UserId = dto.UserId;

            return Id;
        }
        #endregion
    }
}
