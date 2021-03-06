﻿using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using Microsoft.Extensions.Logging;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Implementation of flag service interface.
    /// Saves flags. Does not do anything else yet.
    /// </summary>
    public class FlagService : IFlagService
    {
        private readonly IAnswerDescriptionService _answerDescriptionService;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<AnswerFlag> _answerFlagRepository;
        private readonly IRepository<AnswerDescriptionFlag> _answerDescriptionFlagRepository;
        private readonly ILogger _logger;

        public FlagService(
            IAnswerDescriptionService answerDescriptionService,
            ICacheManager cacheManager, 
            IRepository<AnswerFlag> answerFlagRepository,
            IRepository<AnswerDescriptionFlag> answerDescriptionFlagRepository, 
            ILoggerFactory loggerFactory)
        {
            _answerDescriptionService = answerDescriptionService;
            _cacheManager = cacheManager;
            _answerFlagRepository = answerFlagRepository;
            _answerDescriptionFlagRepository = answerDescriptionFlagRepository;
            _logger = loggerFactory.CreateLogger<FlagService>();
        }

        #region IFlagService implementation
        /// <summary>
        /// Save answer flag
        /// </summary>
        /// <param name="answerFlag"></param>
        /// <returns></returns>
        /// <remarks>Flag is always considered new</remarks>
        public DataOperationResult FlagAnswer(AnswerFlagDto answerFlag)
        {
            if (answerFlag == null)
                throw new ServicesException("Null parameter FlagService.FlagAnswer(answerFlag)");

            if (answerFlag.AnswerId <= 0)
                throw new ServicesException("Unexpected AnswerId in FlagService.FlagAnswer(answerFlag)");

            if (answerFlag.UserId == null)
                throw new ServicesException("Unexpected UserId in FlagService.FlagAnswer(answerFlag)");

            var answerFlagObject = new AnswerFlag();
            answerFlagObject.FromDto(answerFlag);

            _answerFlagRepository.Insert(answerFlagObject);

            var task = _answerFlagRepository.SaveChangesAsync();
            task.Wait();

            // Add to user cache if there is a user
            if (answerFlagObject.UserId != null)
            {
                var userCachedData = GetUserFlagsCachedData();
                userCachedData.Insert(answerFlagObject);
            }

            return new DataOperationResult() { IntId = answerFlagObject.Id, IsNew = true };
        }

        /// <summary>
        /// Save answer description flag
        /// </summary>
        /// <param name="answerFlag"></param>
        /// <returns></returns>
        /// <remarks>Flag is always considered new</remarks>
        public DataOperationResult FlagAnswerDescription(AnswerDescriptionFlagDto answerDescriptionFlag)
        {
            if (answerDescriptionFlag == null)
                throw new ServicesException("Null parameter FlagService.FlagAnswerDescription(answerDescriptionFlag)");

            if (answerDescriptionFlag.AnswerDescriptionId <= 0)
                throw new ServicesException("Unexpected AnswerDescriptionId in FlagService.FlagAnswerDescription(answerDescriptionFlag)");

            if (answerDescriptionFlag.UserId == null)
                throw new ServicesException("Unexpected UserId in FlagService.FlagAnswerDescription(answerDescriptionFlag)");


            var answerDescriptionFlagObject = new AnswerDescriptionFlag();
            answerDescriptionFlagObject.FromDto(answerDescriptionFlag);

            _answerDescriptionFlagRepository.Insert(answerDescriptionFlagObject);

            var task = _answerDescriptionFlagRepository.SaveChangesAsync();
            task.Wait();

            // Add to user cache if there is a user
            if (answerDescriptionFlagObject.UserId != null)
            {
                var userCachedData = GetUserDescriptionFlagsCachedData();
                userCachedData.Insert(answerDescriptionFlagObject);
            }

            // Find the id of the answer whos description was flagged
            // We need to give it caller so that caller can redirect to answer page
            var answerDescriptionDto = _answerDescriptionService
                .FindByAnswerDescriptionId(answerDescriptionFlag.AnswerDescriptionId);

            var result = new DataOperationResult();
            result.IntId = answerDescriptionDto.AnswerId;
            result.IsNew = true;
            return result;
        }

        /// <summary>
        /// Count flags for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CountByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId)) return 0;
            var cachedData = GetUserFlagsCachedData();
            int result = cachedData.Count(userId);
            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get answer flags data from cache or initialize if empty
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerFlag> GetUserFlagsCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_USER_FLAGS_DATA);
            if (data == null)
            {
                var dataSource = new KeyIndexedDataSource<AnswerFlag>();
                dataSource.Initialize(_answerFlagRepository.Active());
                _cacheManager.Add(CacheConstants.CACHE_KEY_USER_FLAGS_DATA, dataSource);
                return dataSource;
            }
            return (KeyIndexedDataSource<AnswerFlag>)data;
        }

        /// <summary>
        /// Get answer description flags data from cache or initialize if empty
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerDescriptionFlag> GetUserDescriptionFlagsCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_USER_DESCRIPTION_FLAGS_DATA);
            if (data == null)
            {
                var dataSource = new KeyIndexedDataSource<AnswerDescriptionFlag>();
                dataSource.Initialize(_answerDescriptionFlagRepository.Active());
                _cacheManager.Add(CacheConstants.CACHE_KEY_USER_DESCRIPTION_FLAGS_DATA, dataSource);
                return dataSource;
            }
            return (KeyIndexedDataSource<AnswerDescriptionFlag>)data;
        }
        #endregion
    }
}
