using BestFor.Dto.Account;
using BestFor.Services.Blobs;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BestFor.ViewComponents
{
    /// <summary>
    /// Shows all top posters.
    /// The view for this view component in Views/Shared/TopPosters
    /// </summary>
    public class TopPostersViewComponent : ViewComponent
    {
        /// <summary>
        /// Constructor injected answer service. Used for loading the answers.
        /// </summary>
        private readonly IAnswerService _answerService;
        private readonly IUserService _userService;
        private readonly IBlobService _blobService;

        public TopPostersViewComponent(IAnswerService answerService, IUserService userService, IBlobService blobService)
        {
            _userService = userService;
            _answerService = answerService;
            _blobService = blobService;
        }

        /// <summary>
        /// This is the main rendering method
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ApplicationUsersDto();

            model.Users = await _answerService.FindTopPosterIds();

            // model.Users are from answers. We need to get the same users from user service
            // because they might have cached images and user statistics
            // get the list of user ids
            var userIds = model.Users.Select(x => x.UserId).ToList();
            // get users from cache
            var cachedUsers = _userService.FindByIds(userIds);

            // Load image for it if needed and reset the models list with these users.
            model.Users = new List<ApplicationUserDto>();
            foreach (var user in cachedUsers)
            {
                _blobService.GetUserImagUrl(user.Value);
                model.Users.Add(user.Value);
            }

            return View("TopPosters", model);
        }
    }
}
