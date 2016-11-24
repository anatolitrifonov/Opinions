using Microsoft.Extensions.Logging;
using BestFor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    /// <summary>
    /// This service has one method that unifies user's statistics update.
    /// Another useful point is that this service allows not to drag all services needed
    /// for updating user statistics into all the places that might trigger the update.
    /// Injection saves us some code writing in this case.
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly IAnswerDescriptionService _answerDescriptionService;
        private readonly IVoteService _voteService;
        private readonly IAnswerService _answerService;
        private readonly IFlagService _flagService;
        private readonly ILogger _logger;

        public StatisticsService(ILoggerFactory loggerFactory,
            IAnswerDescriptionService answerDescriptionService, IVoteService voteService,
            IAnswerService answerService, IFlagService flagService)
        {
            _logger = loggerFactory.CreateLogger<StatisticsService>();
            _logger.LogInformation("created StatisticsService");
            _answerService = answerService;
            _answerDescriptionService = answerDescriptionService;
            _voteService = voteService;
            _flagService = flagService;
        }

        /// <summary>
        /// Update user statistics from different indexes and services if it is not up to date.
        /// </summary>
        /// <param name="user"></param>
        public void LoadUserStatictics(ApplicationUser user)
        {
            // No need to go into cache and read stuff if user is up to date.
            // We hope that we covered all the places that will trigger statistics change.
            if (user.IsStatisticsCached) return;

            user.NumberOfAnswers = _answerService.CountByUserId(user.Id); // user.NumberOfAnswers;
            user.NumberOfDescriptions = _answerDescriptionService.CountByUserId(user.Id); // user.NumberOfDescriptions;
            user.NumberOfVotes = _voteService.CountByUserId(user.Id); // user.NumberOfVotes;
            user.NumberOfFlags = _flagService.CountByUserId(user.Id); // user.NumberOfFlags;

            user.IsStatisticsCached = true;
        }
    }
}
