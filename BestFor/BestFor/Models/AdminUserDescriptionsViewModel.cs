using BestFor.Dto;
using BestFor.Dto.Account;
using System.Collections.Generic;

namespace BestFor.Models
{
    public class AdminUserDescriptionsViewModel
    {
        public ApplicationUserDto User { get; set; }

        public IEnumerable<AnswerDescriptionDto> AnswerDescriptions { get; set; }
    }
}
