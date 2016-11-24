using BestFor.Domain.Entities;
using BestFor.Dto.Account;
using BestFor.Services.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        #endregion

        #region Private Methods
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
                // We  can theoretically do this hear too but let all the answers cache be deal with by answer service.
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
