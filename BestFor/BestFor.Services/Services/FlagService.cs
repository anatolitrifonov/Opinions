using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Implementation of flag service interface.
    /// Saves flags. Does not do anything else yet.
    /// </summary>
    public class FlagService : IFlagService
    {
        private ICacheManager _cacheManager;
        private IRepository<AnswerFlag> _answerFlagRepository;
        private IRepository<AnswerDescriptionFlag> _answerDescriptionFlagRepository;
        private ILogger _logger;

        public FlagService(
            ICacheManager cacheManager, 
            IRepository<AnswerFlag> answerFlagRepository,
            IRepository<AnswerDescriptionFlag> answerDescriptionFlagRepository, 
            ILoggerFactory loggerFactory)
        {
            _cacheManager = cacheManager;
            _answerFlagRepository = answerFlagRepository;
            _answerDescriptionFlagRepository = answerDescriptionFlagRepository;
            _logger = loggerFactory.CreateLogger<FlagService>();
        }

        /// <summary>
        /// Save answer flag
        /// </summary>
        /// <param name="answerFlag"></param>
        /// <returns></returns>
        public int FlagAnswer(AnswerFlagDto answerFlag)
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

            return answerFlagObject.Id;
        }

        /// <summary>
        /// Save answer description flag
        /// </summary>
        /// <param name="answerFlag"></param>
        /// <returns></returns>
        public int FlagAnswerDescription(AnswerDescriptionFlagDto answerDescriptionFlag)
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

            return answerDescriptionFlagObject.Id;
        }
    }
}
