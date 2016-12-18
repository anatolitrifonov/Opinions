using BestFor.Dto;
using BestFor.Dto.Account;
using System.Collections.Generic;
using System.Linq;

namespace BestFor.Services.Services
{
    public class Stitcher<T> where T : UserBaseDto
    {
        /// <summary>
        /// Given the list of Dto's that cam contain user information find this info in cache and populate
        /// user in each item in the list.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userService"></param>
        public static void Stitch(IEnumerable<T> data, IUserService userService)
        {
            // Get users.
            var userIds = data.Where(x => x.UserId != null).Select(x => x.UserId).Distinct().ToList();

            // Get users (as dictionary)
            var users = userService.FindByIds(userIds);

            // Stitch data and users together
            foreach (var item in data)
            {
                if (item.UserId != null)
                {
                    ApplicationUserDto user;
                    if (users.TryGetValue(item.UserId, out user))
                    {
                        item.User = user;
                    }
                }
            }
        }
    }
}
