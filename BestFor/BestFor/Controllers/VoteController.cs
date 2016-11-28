using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BestFor.Controllers
{
    /// <summary>
    /// Allows user to vote for answers and answer descriptions.
    /// 
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Authorize]
    public class VoteController : BaseApiController
    {
        private readonly IUserService _userService;
        private IVoteService _voteService;
        private readonly ILogger _logger;
        private readonly IResourcesService _resourcesService;
        private readonly IStatisticsService _statisticsService;

        public VoteController(IUserService userService, IVoteService voteService, IResourcesService resourcesService,
            ILoggerFactory loggerFactory, IStatisticsService statisticsService)
        {
            _userService = userService;
            _voteService = voteService;
            _resourcesService = resourcesService;
            _logger = loggerFactory.CreateLogger<VoteController>();
            _logger.LogInformation("created VoteController");
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult VoteAnswer(int answerId = 0)
        {
            _logger.LogDebug("VoteAnswer answerId = " + answerId);

            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();
            navigationData.AnswerId = answerId;

            // Only do something is answer id is not zero
            if (answerId <= 0)
                // Redirect to some random answer that might even not exist
                return RedirectToAction("ShowAnswer", "AnswerAction", new { data = NavigationHelper.Encode(navigationData) });

            var user = _userService.FindByUserName(User.Identity.Name);
            // Check if user statistics is loaded
            _statisticsService.LoadUserStatictics(user);

            var voteResult = _voteService.VoteAnswer(
                new AnswerVoteDto() { AnswerId = answerId, UserId = user.Id }
            );

            if (voteResult.IsNew)
            {
                user.NumberOfVotes++;
                navigationData.UserLevelingResult = _userService.LevelUser(user, EventType.AnswerVoteAdded);
            }

            // Read the reason
            navigationData.Reason = _resourcesService.GetString(this.Culture, Lines.THANK_YOU_FOR_VOTING);

            return RedirectToAction("ShowAnswer", "AnswerAction", new { data = NavigationHelper.Encode(navigationData) });
        }

        [HttpGet]
        public IActionResult VoteAnswerDescription(int answerDescriptionId = 0)
        {
            _logger.LogDebug("VoteAnswerDescription answerDescriptionId = " + answerDescriptionId);

            // Only do something is answer id is not zero
            if (answerDescriptionId != 0)
                // Redirect to some random answer that might even not exist
                return RedirectToAction("ShowAnswer", "AnswerAction", new { answerId = answerDescriptionId });

            // this does return answerId
            var user = _userService.FindByUserName(User.Identity.Name);
            // Check if user statistics is loaded
            _statisticsService.LoadUserStatictics(user);
            var voteResult = _voteService.VoteAnswerDescription(
                new AnswerDescriptionVoteDto() { AnswerDescriptionId = answerDescriptionId, UserId = user.Id }
            );

            if (voteResult.IsNew)
            {
                user.NumberOfVotes++;
                _userService.LevelUser(user, EventType.AnswerDescriptionVoteAdded);
            }

            // Read the reason
            var reason = _resourcesService.GetString(this.Culture, Lines.THANK_YOU_FOR_VOTING);

            return RedirectToAction("ShowAnswer", "AnswerAction", new { answerId = voteResult.IntId, reason = reason });
        }
    }
}
