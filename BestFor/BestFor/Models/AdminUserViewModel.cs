using BestFor.Dto;
using BestFor.Dto.Account;
using System.Collections.Generic;

namespace BestFor.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUserDto User { get; set; }

        public IEnumerable<AnswerDto> Answers { get; set; }
    }
}
