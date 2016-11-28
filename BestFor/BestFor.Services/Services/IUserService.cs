using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Dto.Account;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Interface for user service.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// In our world display name is supposed to be unique. This method helps finding user by display name.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        ApplicationUser FindByDisplayName(string displayName);

        ApplicationUser FindById(string id);

        ApplicationUser FindByUserName(string userName);

        /// <summary>
        /// Find users by a set of ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Dictionary<string, ApplicationUserDto> FindByIds(List<string> ids);

        /// <summary>
        /// List all users
        /// </summary>
        /// <returns></returns>
        IEnumerable<ApplicationUser> FindAll();

        int AddUserToCache(ApplicationUser user);

        /// <summary>
        /// Verifies if user needs to get a level.
        /// Updates user's level.
        /// Verifies user achievements.
        /// Updates user's achiements list.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="eventType"></param>
        UserLevelingResultDto LevelUser(ApplicationUser user, EventType eventType);
    }
}
