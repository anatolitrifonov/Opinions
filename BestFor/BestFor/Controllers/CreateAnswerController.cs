using BestFor.Dto;
using BestFor.Models;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BestFor.Controllers
{
    /// <summary>
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// Controller itself allows answer creation from an MVC page (not from react page).
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Authorize]
    public class CreateAnswerController : BaseApiController
    {
        private readonly IAnswerService _answerService;
        private readonly IProfanityService _profanityService;
        private readonly ISuggestionService _suggestionService;
        private readonly IUserService _userService;
        private readonly IAntiforgery _antiforgery;
        private readonly IStatisticsService _statisticsService;
        private readonly IResourcesService _resourcesService;
        private readonly ILogger _logger;

        public CreateAnswerController(IAnswerService answerService, IProfanityService profanityService,
            ISuggestionService suggestionService, IUserService userService, IAntiforgery antiforgery,
            IStatisticsService statisticsService, IResourcesService resourcesService,
            ILoggerFactory loggerFactory)
        {
            _answerService = answerService;
            _profanityService = profanityService;
            _suggestionService = suggestionService;
            _userService = userService;
            _antiforgery = antiforgery;
            _statisticsService = statisticsService;
            _resourcesService = resourcesService;
            _logger = loggerFactory.CreateLogger<CreateAnswerController>();
            _logger.LogInformation("created CreateAnswerController");
        }

        /// <summary>
        /// Add description for the answer.
        /// todo: figure out how to protect from spam posts
        /// </summary>
        /// <param name="answerDescription"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult AddAnswer(AddAnswerViewModel addAnswerViewModel)
        {
            // Basic checks first
            if (addAnswerViewModel == null) return View("Error");
            if (!ModelState.IsValid) return View("Error");

            // Create answer controller because we will just use it
            var answerController = new AnswerController(_answerService, _profanityService, _suggestionService,
                _userService, _antiforgery, _statisticsService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = this.HttpContext
                }
            };

            // Create object to post
            var answer = new AnswerDto()
            {
                LeftWord = addAnswerViewModel.LeftWord,
                RightWord = addAnswerViewModel.RightWord,
                Phrase = addAnswerViewModel.Phrase,
                UserId = this.GetUserId(this.User, _userService)
            };

            // Roll the dice and post.
            var task = answerController.AddAnswer(answer);
            task.Wait();
            var resultAnswer = task.Result;

            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();

            // Read the reason to return it to UI.
            // All good
            if (resultAnswer.ErrorMessage == null)
            {
                navigationData.Reason = _resourcesService.GetString(this.Culture, Lines.OPINION_WAS_ADDED_SUCCESSFULLY);
                navigationData.AnswerId = resultAnswer.Answer.Id;
                navigationData.UserLevelingResult = resultAnswer.UserLevelingResult;

                // Redirect to show the answer. This will prevent user refreshing the page.
                return RedirectToAction("ShowAnswer", "AnswerAction", new { data = NavigationHelper.Encode(navigationData) });
            }
            // All bad
            else
            {
                navigationData.Reason = string.Format(_resourcesService.GetString(this.Culture, Lines.OPINION_WAS_NOT_ADDED), resultAnswer.ErrorMessage);
                return RedirectToAction("Index", "Home", new { reason = navigationData.Reason });
            }
        }
    }
}
