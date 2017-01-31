using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Implementation of vote service interface.
    /// Saves votes. Does not do anything else yet.
    /// </summary>
    /// <remarks>Reason for not combining this with answer service is so that it can be tested separately
    /// and not to load the classes to much</remarks>
    public class HelpItemService : IHelpItemService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<HelpItem> _helpItemRepository;
        private readonly ILogger _logger;

        public HelpItemService(
            ICacheManager cacheManager, 
            IRepository<HelpItem> helpItemRepository,
            ILoggerFactory loggerFactory)
        {
            _cacheManager = cacheManager;
            _helpItemRepository = helpItemRepository;
            _logger = loggerFactory.CreateLogger<VoteService>();
        }

        #region IHelpItemService implementation
        public IEnumerable<HelpItemDto> FindAll(string culture)
        {
            if (string.IsNullOrEmpty(culture)) culture = ResourcesService.DEFAULT_CULTURE;

            // Get cache.
            var cachedData = GetHelpItemsCachedData();

            return cachedData.Where(x => x.CultureName == culture);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get votes data from cache or initialize to just a list of Dtos if empty
        /// </summary>
        /// <returns></returns>
        private List<HelpItemDto> GetHelpItemsCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_HELP_DATA);
            if (data == null)
            {
                var dbData = _helpItemRepository.List().Select(x => x.ToDto()).ToList();
                _cacheManager.Add(CacheConstants.CACHE_KEY_HELP_DATA, dbData);
                return dbData;
            }
            return (List<HelpItemDto>)data;
        }
        #endregion
    }
}
