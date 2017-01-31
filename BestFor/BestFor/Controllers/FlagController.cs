using BestFor.Dto;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BestFor.Controllers
{
    /// <summary>
    /// Allows user to flag data if he sees something wrong with it.
    /// 
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Authorize]
    public class FlagController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IFlagService _flagService;
        private readonly ILogger _logger;
        private readonly IResourcesService _resourcesService;
        private readonly IStatisticsService _statisticsService;

        public FlagController(IUserService userService, IFlagService flagService, IResourcesService resourcesService,
            ILoggerFactory loggerFactory, IStatisticsService statisticsService)
        {
            _flagService = flagService;
            _resourcesService = resourcesService;
            _logger = loggerFactory.CreateLogger<FlagController>();
            _userService = userService;
            _logger.LogInformation("created FlagController");
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public IActionResult FlagAnswer(int answerId = 0)
        {
            _logger.LogDebug("FlagAnswer answerId = " + answerId);

            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();
            navigationData.AnswerId = answerId;

            // Only do something if answer id is not zero
            if (answerId <= 0)
                // Redirect to some random answer that might even not exist
                return RedirectToAction("ShowAnswer", "AnswerAction", new { data = NavigationHelper.Encode(navigationData) });

            var user = _userService.FindByUserName(User.Identity.Name);
            // Check if user statistics is loaded
            _statisticsService.LoadUserStatictics(user);

            var result = _flagService.FlagAnswer(
                new AnswerFlagDto() { AnswerId = answerId, UserId = user.UserId } 
            );

            if (result.IsNew)
            {
                user.NumberOfFlags++;
                navigationData.UserLevelingResult = _userService.LevelUser(user, EventType.AnswerFlagAdded);
            }

            // Read the reason
            navigationData.Reason = _resourcesService.GetString(this.Culture, Lines.THANK_YOU_FOR_FLAGING);

            return RedirectToAction("ShowAnswer", "AnswerAction", new { data = NavigationHelper.Encode(navigationData) });
        }

        [HttpGet]
        public IActionResult FlagAnswerDescription(int answerDescriptionId = 0)
        {
            _logger.LogDebug("FlagAnswerDescription answerDescriptionId = " + answerDescriptionId);

            // Only do something is answer id is not zero
            if (answerDescriptionId <= 0)
                // Redirect to some random answer that might even not exist
                return RedirectToAction("ShowAnswer", "AnswerAction", new { answerId = answerDescriptionId });

            var user = _userService.FindByUserName(User.Identity.Name);
            // Check if user statistics is loaded
            _statisticsService.LoadUserStatictics(user);
            
            // this does return answerId
            var result = _flagService.FlagAnswerDescription(
                new AnswerDescriptionFlagDto() { AnswerDescriptionId = answerDescriptionId, UserId = user.UserId }
            );

            if (result.IsNew)
            {
                user.NumberOfFlags++;
                _userService.LevelUser(user, EventType.AnswerDescriptionFlagAdded);
            }

            // Read the reason
            var reason = _resourcesService.GetString(this.Culture, Lines.THANK_YOU_FOR_FLAGING);

            return RedirectToAction("ShowAnswer", "AnswerAction", new { answerId = result.IntId, reason = reason });
        }
    }
}
