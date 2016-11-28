using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Dto.Account;
using BestFor.Services.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Implements additional operations with users. This service helps keeping user stats up to date
    /// and returns achivement notification on any stats related updates.
    /// </summary>
    public class UserService : IUserService
    {
        private ILogger _logger;
        private UserManager<ApplicationUser> _userManager;
        private ICacheManager _cacheManager;
        //private readonly IStatisticsService _statisticsService;

        public UserService(UserManager<ApplicationUser> userManager, ILoggerFactory loggerFactory, ICacheManager cacheManager)//,
            //IStatisticsService statisticsService)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<VoteService>();
            _logger.LogInformation("created UserService");
            _cacheManager = cacheManager;
            //_statisticsService = statisticsService;
        }

        #region IUserService implementation
        /// <summary>
        /// This one loads straight from the database
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public ApplicationUser FindByDisplayName(string displayName)
        {
            // Do not check nulls
            if (string.IsNullOrEmpty(displayName)) return null;
            if (string.IsNullOrWhiteSpace(displayName)) return null;

            // var// var

            // Find first user with this display name. 
            return _userManager.Users.FirstOrDefault(x => x.DisplayName == displayName);
        }

        /// <summary>
        /// This one is not 100% solid. If user is not in cache, null will be returned.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApplicationUser FindById(string id)
        {
            // Do not check nulls
            if (string.IsNullOrEmpty(id)) return null;
            if (string.IsNullOrWhiteSpace(id)) return null;

            // Get cache ... might take long but is not null
            var data = GetCachedData();

            ApplicationUser user;
            if (data.TryGetValue(id, out user))
                return user;

            return null;
        }

        public ApplicationUser FindByUserName(string userName)
        {
            // Do not check nulls
            if (string.IsNullOrEmpty(userName)) return null;
            if (string.IsNullOrWhiteSpace(userName)) return null;

            // Get cache ... might take long but is not null
            var data = GetCachedData();

            ApplicationUser user = data.FirstOrDefault(x => x.Value.UserName == userName).Value;

            return user;
        }

        /// <summary>
        /// Find users by a set of ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Dictionary<string, ApplicationUserDto> FindByIds(List<string> ids)
        {
            // Assume that ids list is distinct. We are not going to do distinct on it.
            // if we run into issues because of that we will add distinct.

            // Do check nulls
            if (ids == null) return null;

            // Get cache ... might take long but is not null
            var data = GetCachedData();

            var result = new Dictionary<string, ApplicationUserDto>();

            // Build a list of users from ids.
            ApplicationUser user;
            foreach (var id in ids)
            {
                if (data.TryGetValue(id, out user))
                {
                    if (!result.ContainsKey(id))
                        result.Add(id, user.ToDto());
                }
            }
            return result;
        }

        /// <summary>
        /// Find all users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> FindAll()
        {
            var data = GetCachedData();

            var result = data.Values.AsEnumerable<ApplicationUser>();

            return result;
        }

        /// <summary>
        /// Cache user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUserToCache(ApplicationUser user)
        {
            // load cache
            Dictionary<string, ApplicationUser> data = GetCachedData();
            // Something went wrong if this is null.
            // if (data == null) return 0;

            if (data.ContainsKey(user.Id))
                data[user.Id] = user;
            else
                data.Add(user.Id, user);

            return 1;
        }

        private enum UserLevels: int
        {
            Level1 = 10,
            Level2 = 20,
            Level3 = 50,
            Level4 = 100,
            Level5 = 500,
            Level6 = 2000,
            Level7 = 5000,
            Level8 = 10000,
            Level9 = 20000,
            Level10 = 50000,
            Level11 = 100000
        }

        /// <summary>
        /// Simple implementation of user's leveling.
        /// </summary>
        /// <param name="user"></param>
        public UserLevelingResultDto LevelUser(ApplicationUser user, EventType eventType)
        {
            var result = new UserLevelingResultDto();
            // Let's do levels as UserLevels 10, 100, 200, 500, 1000, 2000, 
            // Collect all combines stats
            int totalStats = user.NumberOfAnswers + user.NumberOfDescriptions + user.NumberOfVotes + user.NumberOfFlags;
            // Check if user's stats are up to date. Would be crazy if they are not.
            if (!user.IsStatisticsCached)
                throw new ServicesException("Can not level user without updated statistics");
            // Check if user's level corresponds to where he is at
            var levels = GetLevels();
            int newLevel = 0;
            for (int i = 0; i < levels.Length - 1; i++)
            {
                if (totalStats < levels[i])
                {
                    newLevel = i;
                    break;
                }
            }
            if (newLevel > user.Level)
            {
                // User's level changed.
                user.Level = newLevel;
                result.GainedLevel = true;
                result.Level = newLevel;
                UpdateUserLevel(user);
            }
            return result;
        }
        #endregion

        #region Private Methods
        private void UpdateUserLevel(ApplicationUser user)
        {
            // Load user to update
            var taskToLoadUser = _userManager.FindByIdAsync(user.Id);
            taskToLoadUser.Wait();
            var userToUpdate = taskToLoadUser.Result;

            userToUpdate.Level = user.Level;

            var taskToUpdateUser = _userManager.UpdateAsync(userToUpdate);
            taskToUpdateUser.Wait();
            var identityResult = taskToUpdateUser.Result;
            
            //TODO check update result.
            //TODO Redo caching into user dto
        }

        /// <summary>
        /// Do not want to unstantiate this unless an event it happening that is worth instantiating this array.
        /// </summary>
        /// <returns></returns>
        private int[] GetLevels()
        {
            return new int[]
            {
                (int)UserLevels.Level1,
                (int)UserLevels.Level2,
                (int)UserLevels.Level3,
                (int)UserLevels.Level4,
                (int)UserLevels.Level5,
                (int)UserLevels.Level6,
                (int)UserLevels.Level7,
                (int)UserLevels.Level8,
                (int)UserLevels.Level9,
                (int)UserLevels.Level10,
                (int)UserLevels.Level11
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>The knows problem with this in that when my content is rendered
        /// this function is called for each answer description. Need to optimize somehow.</remarks>
        private Dictionary<string, ApplicationUser> GetCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_USERS_DATA);
            if (data == null)
            {
                var dataSource = new Dictionary<string, ApplicationUser>();

                // We are not going to load the number of answers per user.
                // We will let answer service to deal with this.
                // We will store index of all user answers there. Not here.
                // We can theoretically do this hear too but let all the answers cache be deal with by answer service.
                foreach (var user in _userManager.Users)
                    dataSource.Add(user.Id, user);

                _cacheManager.Add(CacheConstants.CACHE_KEY_USERS_DATA, dataSource);
                return dataSource;
            }
            return (Dictionary<string, ApplicationUser>)data;
        }
        #endregion
    }
}
