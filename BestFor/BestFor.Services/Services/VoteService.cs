using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using BestFor.Services.DataSources;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Implementation of vote service interface.
    /// Saves votes. Does not do anything else yet.
    /// </summary>
    /// <remarks>Reason for not combining this with answer service is so that it can be tested separately
    /// and not to load the classes to much</remarks>
    public class VoteService : IVoteService
    {
        private readonly IAnswerDescriptionService _answerDescriptionService;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<AnswerVote> _answerVoteRepository;
        private readonly IRepository<AnswerDescriptionVote> _answerDescriptionVoteRepository;
        private readonly ILogger _logger;

        public VoteService(
            IAnswerDescriptionService answerDescriptionService,
            ICacheManager cacheManager, 
            IRepository<AnswerVote> answerVoteRepository,
            IRepository<AnswerDescriptionVote> answerDescriptionVoteRepository, 
            ILoggerFactory loggerFactory)
        {
            _answerDescriptionService = answerDescriptionService;
            _cacheManager = cacheManager;
            _answerVoteRepository = answerVoteRepository;
            _answerDescriptionVoteRepository = answerDescriptionVoteRepository;
            _logger = loggerFactory.CreateLogger<VoteService>();
        }

        #region IVoteService implementation
        /// <summary>
        /// Save answer vote
        /// </summary>
        /// <param name="voteVote"></param>
        /// <returns>Id of the answer whos description was voted.</returns>
        public DataOperationResult VoteAnswer(AnswerVoteDto answerVote)
        {
            if (answerVote == null)
                throw new ServicesException("Null parameter VoteService.VoteAnswer(answerVote)");

            if (answerVote.AnswerId <= 0)
                throw new ServicesException("Unexpected AnswerId in VoteService.VoteAnswer(answerVote)");

            if (answerVote.UserId == null)
                throw new ServicesException("Unexpected UserId in VoteService.VoteAnswer(answerVote)");

            var result = new DataOperationResult();

            // Find if vote is already there.
            var existingVote = _answerVoteRepository.Queryable()
                .FirstOrDefault(x => x.UserId == answerVote.UserId && x.AnswerId == answerVote.AnswerId);
            // Do not re-add existing vote.
            if (existingVote != null)
            {
                result.IntId = existingVote.AnswerId;
                return result;
            }

            // Add new answer vote
            var answerVoteObject = new AnswerVote();
            answerVoteObject.FromDto(answerVote);

            _answerVoteRepository.Insert(answerVoteObject);
            var task = _answerVoteRepository.SaveChangesAsync();
            task.Wait();

            // Add to cache.
            var cachedData = GetVotesCachedData();
            cachedData.Insert(answerVoteObject);

            // Add to user cache if there is a user
            if (answerVoteObject.UserId != null)
            {
                var userCachedData = GetUserVotesCachedData();
                userCachedData.Insert(new AnswerVoteUserMask(answerVoteObject));
            }

            result.IntId = answerVoteObject.AnswerId;
            result.IsNew = true;
            return result;
        }

        /// <summary>
        /// Save answer description Vote
        /// </summary>
        /// <param name="answerVote"></param>
        /// <returns>Id of the answer whos description was voted.</returns>
        public DataOperationResult VoteAnswerDescription(AnswerDescriptionVoteDto answerDescriptionVote)
        {
            if (answerDescriptionVote == null)
                throw new ServicesException("Null parameter VoteService.VoteAnswerDescription(answerDescriptionVote)");

            if (answerDescriptionVote.AnswerDescriptionId <= 0)
                throw new ServicesException("Unexpected AnswerDescriptionId in VoteService.VoteAnswerDescription(answerDescriptionVote)");

            if (answerDescriptionVote.UserId == null)
                throw new ServicesException("Unexpected UserId in VoteService.VoteAnswerDescription(answerDescriptionVote)");

            var result = new DataOperationResult();

            // Find if vote is already there.
            var existingVote = _answerDescriptionVoteRepository.Queryable()
                .FirstOrDefault(x => x.UserId == answerDescriptionVote.UserId && x.AnswerDescriptionId == answerDescriptionVote.AnswerDescriptionId);
            // Do not re-add existing vote.
            if (existingVote != null)
            {
                result.IntId = existingVote.Id;
                return result;
            }

            // Add new description vote
            var answerDescriptionVoteObject = new AnswerDescriptionVote();
            answerDescriptionVoteObject.FromDto(answerDescriptionVote);

            // Insert
            _answerDescriptionVoteRepository.Insert(answerDescriptionVoteObject);
            var task = _answerDescriptionVoteRepository.SaveChangesAsync();
            task.Wait();

            // Add to cache.
            var cachedData = GetVoteDescriptionsCachedData();
            cachedData.Insert(answerDescriptionVoteObject);

            // Add to user cache if there is a user
            if (answerDescriptionVoteObject.UserId != null)
            {
                var userCachedData = GetUserVoteDescriptionsCachedData();
                userCachedData.Insert(new AnswerDescriptionVoteUserMask(answerDescriptionVoteObject));
            }

            // Find the id of the answer whos description was voted for
            var answerDescriptionDto = _answerDescriptionService
                .FindByAnswerDescriptionId(answerDescriptionVote.AnswerDescriptionId);

            result.IntId = answerDescriptionDto.AnswerId;
            result.IsNew = true;
            return result;
        }

        public int CountAnswerVotes(int answerId)
        {
            if (answerId <= 0) return 0;

            // Get cache.
            var cachedData = GetVotesCachedData();
            // Get count from cache.
            var data = cachedData.Find(answerId.ToString());
            // This will never happen because cache will get reinitialized if null
            if (data == null) return 0;
            return data.Count();

            // int count = _answerVoteRepository.Queryable().Count(x => x.AnswerId == answerId);
            // return await Task.FromResult<int>(count);
        }

        /// <summary>
        /// Count votes for a given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int CountByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(userId)) return 0;
            var cachedData = GetUserVotesCachedData();
            int result = cachedData.Count(userId);
            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get votes data from cache or initialize if empty
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerVote> GetVotesCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_VOTES_DATA);
            if (data == null)
            {
                var dataSource = new KeyIndexedDataSource<AnswerVote>();
                dataSource.Initialize(_answerVoteRepository.Active());
                _cacheManager.Add(CacheConstants.CACHE_KEY_VOTES_DATA, dataSource);
                return dataSource;
            }
            return (KeyIndexedDataSource<AnswerVote>)data;
        }

        /// <summary>
        /// Get vote descriptions data from cache or initialize if empty
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerDescriptionVote> GetVoteDescriptionsCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_DESCRIPTION_VOTES_DATA);
            if (data == null)
            {
                var dataSource = new KeyIndexedDataSource<AnswerDescriptionVote>();
                dataSource.Initialize(_answerDescriptionVoteRepository.Active());
                _cacheManager.Add(CacheConstants.CACHE_KEY_DESCRIPTION_VOTES_DATA, dataSource);
                return dataSource;
            }
            return (KeyIndexedDataSource<AnswerDescriptionVote>)data;
        }

        /// <summary>
        /// We are doubling the data to give ability to index answer description in a different way.
        /// Main index in on answers.
        /// This will allow us to also index answer descriptions by user.
        /// 
        /// Load from normally cached data if empty.
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerVoteUserMask> GetUserVotesCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_USER_VOTES_DATA);
            if (data == null)
            {
                // Initialize from answer descriptions
                var dataSource = GetVotesCachedData();
                var allItems = dataSource.All();
                var userDataSource = new KeyIndexedDataSource<AnswerVoteUserMask>();
                userDataSource.Initialize(allItems
                    .Where(x => x.UserId != null)
                    .Select(x => new AnswerVoteUserMask(x)));
                _cacheManager.Add(CacheConstants.CACHE_KEY_USER_VOTES_DATA, userDataSource);
                return userDataSource;
            }
            return (KeyIndexedDataSource<AnswerVoteUserMask>)data;
        }

        /// <summary>
        /// We are doubling the data to give ability to index answer description in a different way.
        /// Main index in on answers.
        /// This will allow us to also index answer descriptions by user.
        /// 
        /// Load from normally cached data if empty.
        /// </summary>
        /// <returns></returns>
        private KeyIndexedDataSource<AnswerDescriptionVoteUserMask> GetUserVoteDescriptionsCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_USER_DESCRIPTION_VOTES_DATA);
            if (data == null)
            {
                // Initialize from answer descriptions
                var dataSource = GetVoteDescriptionsCachedData();
                var allItems = dataSource.All();
                var userDataSource = new KeyIndexedDataSource<AnswerDescriptionVoteUserMask>();
                userDataSource.Initialize(allItems
                    .Where(x => x.UserId != null)
                    .Select(x => new AnswerDescriptionVoteUserMask(x)));
                _cacheManager.Add(CacheConstants.CACHE_KEY_USER_DESCRIPTION_VOTES_DATA, userDataSource);
                return userDataSource;
            }
            return (KeyIndexedDataSource<AnswerDescriptionVoteUserMask>)data;
        }
        #endregion
    }
}
