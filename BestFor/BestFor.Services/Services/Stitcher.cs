using BestFor.Domain.Interfaces;
using BestFor.Dto;
using BestFor.Dto.Account;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    public class Stitcher<T> where T : UserBaseDto
    {
        /// <summary>
        /// Stitches users into list of UserBaseDto objects.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userIds"></param>
        /// <param name="userService"></param>
        /// <returns></returns>
        public static List<T> Stitch(List<IDtoConvertable<T>> data, List<string> userIds, IUserService userService)
        {
            // Get users (as dictionary)
            var users = userService.FindByIds(userIds);
            // Stitch data and users together
            var dataWithUsers = new List<T>();
            foreach (var item in data)
            {
                var resultItem = item.ToDto();
                if (resultItem.UserId != null)
                {
                    ApplicationUserDto user;
                    if (users.TryGetValue(resultItem.UserId, out user))
                    {
                        resultItem.User = user;
                    }
                }
                dataWithUsers.Add(resultItem);
            }
            return dataWithUsers;
        }
    }
}
