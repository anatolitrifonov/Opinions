using BestFor.Models;
using BestFor.Services.Blobs;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BestFor.Controllers
{
    /// <summary>
    /// Shows user's public profile.
    /// 
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    public class PublicProfileController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        private readonly IResourcesService _resourcesService;
        private readonly IBlobService _blobService;

        public PublicProfileController(IUserService userService, IResourcesService resourcesService, ILoggerFactory loggerFactory,
            IBlobService blobService)
        {
            _userService = userService;
            _resourcesService = resourcesService;
            _logger = loggerFactory.CreateLogger<VoteController>();
            _logger.LogInformation("created VoteController");
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult Index(string userName)
        {
            _logger.LogDebug("PublicProfileController userName = " + userName);

            var model = new ProfileEditViewModel();

            model.UserName = userName;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrWhiteSpace(userName))
            {
                model.NotFound = true;
                return View(model);
            }

            var user = _userService.FindByUserName(userName);
            if (user == null)
            {
                model.NotFound = true;
                return View(model);
            }

            model.UserImageUrl = _blobService.GetUserImagUrl(user);

            // Fill in the model. Cache is hopefully up to date.
            model.Email = user.Email;
            model.DisplayName = user.DisplayName;
            model.NumberOfAnswers = user.NumberOfAnswers;
            model.NumberOfDescriptions = user.NumberOfDescriptions;
            model.NumberOfVotes = user.NumberOfVotes;
            model.NumberOfFlags = user.NumberOfFlags;
            model.NumberOfComments = user.NumberOfComments;
            model.JoinDate = user.DateAdded;
            model.PhoneNumber = user.PhoneNumber;
            model.CompanyName = user.CompanyName;
            model.WebSite = user.WebSite;
            model.UserDescription = user.UserDescription;
            model.ShowEmail = user.ShowEmail;
            model.ShowPhoneNumber = user.ShowPhoneNumber;
            model.ShowCompanyName = user.ShowCompanyName;
            model.ShowWebSite = user.ShowWebSite;
            model.ShowUserDescription = user.ShowUserDescription;
            model.ShowAvatar = user.ShowAvatar;

            return View(model);
        }
    }
}
