using BestFor.Common;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Models;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BestFor.Controllers
{
    /// <summary>
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// Controller itself adds extended descriptions to the answers.
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Authorize]
    public class AnswerActionController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IAnswerDescriptionService _answerDescriptionService;
        private readonly IAnswerService _answerService;
        private readonly IProfanityService _profanityService;
        private readonly IResourcesService _resourcesService;
        private readonly ILogger _logger;
        private readonly IVoteService _voteService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IStatisticsService _statisticsService;

        public AnswerActionController(IAnswerDescriptionService answerDescriptionService,
            IProfanityService profanityService, IAnswerService answerService, IResourcesService resourcesService, IUserService userService,
            IVoteService voteService, ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, IStatisticsService statisticsService)
        {
            _userService = userService;
            _answerDescriptionService = answerDescriptionService;
            _profanityService = profanityService;
            _answerService = answerService;
            _resourcesService = resourcesService;
            _voteService = voteService;
            _appSettings = appSettings;
            _statisticsService = statisticsService;
            _logger = loggerFactory.CreateLogger<HomeController>();
            _logger.LogInformation("created AnswerActionController");
        }

        /// <summary>
        /// Loads an answer by id to show full details.
        /// </summary>
        /// <param name="answerId"></param>
        /// <param name="reason">Additional details on what happned to answer before we got redirected here.</param>
        /// <returns></returns>
        /// <remarks>We can do ShowAnswer(SomeModel blah) where ShowModel has publis properties Prop1 and Prop2
        /// Or do ShowAnswer(Prop1, Prop2) It will be the same thing since for get we need to pass them in 
        /// URL anyway as ?prop1=z&prop2=x.
        /// 
        /// A bunch of actions will redirect to this one to show the answer after an action does with it.
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ShowAnswer(string data = null)
        {
            // Data can't be null because at least answer id is there.
            var navigationData = NavigationHelper.Decode(data);
            if (navigationData == null) return RedirectToAction("Index", "Home");

            var culture = this.Culture;
            // Load the answer.
            var answer = await _answerService.FindByAnswerId(navigationData.AnswerId);
            // Go home is not found
            if (answer == null) return RedirectToAction("Index", "Home");

            var answerDetailsDto = HomeController.FillInDetails(answer, _answerDescriptionService, _userService, _voteService,
                _resourcesService, culture, _appSettings.Value.FullDomainAddress);

            // Set the reason to be shown on the page in case someone sent it
            answerDetailsDto.Reason = navigationData.Reason;

            // Set the user leveling data in case we need to show level or achievement.
            answerDetailsDto.UserLevelingResult = navigationData.UserLevelingResult;

            return View("MyContent", answerDetailsDto);
        }

        #region Answer Description Actions
        /// <summary>
        /// Load view to add answer description.
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AddDescription(int answerId = 0)
        {
            // Let's load the answer.
            // The hope is that service will not have to go to the database and load answer from cache.
            // But please look at the servise implementation for details.
            var answer = await _answerService.FindByAnswerId(answerId);

            // Model is basically empty at this point.
            var model = new AnswerDescriptionDto() { Answer = answer, AnswerId = answerId };

            return View(model);
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
        public IActionResult AddDescription(AnswerDescriptionDto answerDescription)
        {
            // Basic checks first
            if (answerDescription == null || answerDescription.AnswerId <= 0 ||
                string.IsNullOrEmpty(answerDescription.Description) ||
                string.IsNullOrWhiteSpace(answerDescription.Description)) return View("Error");

            // todo: figure out how to protect from spam posts besides antiforgery

            // cleanup the input
            answerDescription.Description = Services.TextCleaner.Clean(answerDescription.Description);
            // Clean up the endings
            answerDescription.Description = answerDescription.Description.TrimEnd(new Char[] { ' ', '\n', '\r' });

            // Let's first check for profanities.
            var profanityCheckResult = _profanityService.CheckProfanity(answerDescription.Description, this.Culture);
            if (profanityCheckResult.HasIssues)
            {
                // answer.ErrorMessage = profanityCheckResult.ErrorMessage;
                // todo: settle on displaying errors from controller posts and gets
                // Can't use error message from profanity object because it is not localized.
                // Have to localize ourself
                // Might have to move this to profanity service.
                var errorData = new ErrorViewModel();
                errorData.AddError(profanityCheckResult.ErrorMessage);

                return View("Error", errorData);
            }

            // Save the user in case we need statistics update
            ApplicationUser user = null;
            // Load user if he is logged in
            if (User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                user = _userService.FindByUserName(User.Identity.Name);
            }
            // Set the user id in the answer if user is found
            if (user != null)
            {
                answerDescription.UserId = user.Id;
                // Check if user statistics is loaded
                _statisticsService.LoadUserStatictics(user);
            }

            // Add answer description
            var operationResult = _answerDescriptionService.AddAnswerDescription(answerDescription);

            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();

            // Need to update user stats if new answer was added
            if (operationResult.IsNew && user != null)
            {
                user.NumberOfDescriptions++;
                navigationData.UserLevelingResult = _userService.LevelUser(user, EventType.AnswerDescriptionAdded);
            }

            // Read the reason to return it to UI.
            navigationData.Reason = _resourcesService.GetString(this.Culture, Lines.DESCRIPTION_WAS_ADDED_SUCCESSFULLY);
            navigationData.AnswerId = answerDescription.AnswerId;

            // Redirect to show the answer. This will prevent user refreshing the page.
            return RedirectToAction("ShowAnswer", new { data = NavigationHelper.Encode(navigationData) });
        }
        #endregion

        #region Edit Answer Actions
        /// <summary>
        /// Load view to add answer description.
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();
            navigationData.AnswerId = id;
            var nav = new { data = NavigationHelper.Encode(navigationData) };

            if (id <= 0)
                return RedirectToAction("ShowAnswer", nav);

            // Let's load the answer.
            // The hope is that service will not have to go to the database and load answer from cache.
            // But please look at the servise implementation for details.
            var answer = await _answerService.FindByAnswerId(id);

            // No user, no reason to edit.
            if (answer.UserId == null)
                return RedirectToAction("ShowAnswer", nav);

            // Not logged in, can not edit
            // Save the user in case we need statistics update
            ApplicationUser user = null;
            // Load user if he is logged in
            if (User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                user = _userService.FindByUserName(User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("ShowAnswer", nav);

            // Kick them if answer was added not by the current user
            // This prevents going directly to the answer
            if (answer.UserId != user.Id)
                return RedirectToAction("ShowAnswer", nav);

            return View(answer);
        }

        /// <summary>
        /// Add description for the answer.
        /// todo: figure out how to protect from spam posts
        /// </summary>
        /// <param name="answerDescription"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnswerDto answer)
        {
            // Basic checks first
            if (answer == null || answer.Id <= 0) return View("Error");

            // Create the object for passing data between controllers.
            var navigationData = new NavigationDataDto();
            navigationData.AnswerId = answer.Id;
            var nav = new { data = NavigationHelper.Encode(navigationData) };

            if (!ModelState.IsValid)
            {
                return RedirectToAction("ShowAnswer", nav);
            }

            // Find the answer that needs changes
            var answerToModify = await _answerService.FindByAnswerId(answer.Id);

            // No user, no reason to edit.
            if (answerToModify.UserId == null)
                return RedirectToAction("ShowAnswer", nav);

            // Not logged in, can not edit
            // Save the user in case we need statistics update
            ApplicationUser user = null;
            // Load user if he is logged in
            if (User.Identity.IsAuthenticated && User.Identity.Name != null)
            {
                user = _userService.FindByUserName(User.Identity.Name);
            }
            if (user == null)
                return RedirectToAction("ShowAnswer", nav);

            // Kick them if answer was added not by the current user
            // This prevents going directly to the answer
            if (answerToModify.UserId != user.Id)
            {
                navigationData.AnswerId = answerToModify.Id;
                nav = new { data = NavigationHelper.Encode(navigationData) };
                return RedirectToAction("ShowAnswer", nav);
            }

            // Compare the categories because so far this is all we can edit.
            string newCategory = (answer.Category + "").ToLower();
            string oldCategory = (answerToModify.Category + "").ToLower();

            // only do update if changed
            if (oldCategory != newCategory)
            {
                answerToModify.Category = answer.Category;

                // Update answer
                var updatedAnswer = await _answerService.UpdateAnswer(answerToModify);
            }

            // Read the reason
            navigationData.Reason = _resourcesService.GetString(this.Culture, Lines.THANK_YOU_FOR_IMPROVING);

            // Redirect to show the answer. This will prevent user refreshing the page.
            return RedirectToAction("ShowAnswer", new { data = NavigationHelper.Encode(navigationData) });
        }
        #endregion

    }
}
