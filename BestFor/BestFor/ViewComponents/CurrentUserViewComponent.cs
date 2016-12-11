using BestFor.Dto.Account;
using BestFor.Services.Blobs;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BestFor.ViewComponents
{
    /// <summary>
    /// Shows currently logged in user.
    /// The view for this view component in Views/Shared/CurrentUser
    /// 
    /// Compnonents are good replacements for partial views because they can load their own model
    /// When using partial view each page has to provide data for it.
    /// Component allows loading its own model.
    /// </summary>
    public class CurrentUserViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IBlobService _blobService;

        public CurrentUserViewComponent(IUserService userService, IBlobService blobService)
        {
            _userService = userService;
            _blobService = blobService;
        }

        /// <summary>
        /// This is the main rendering method
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ApplicationUserDto model = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = _userService.FindByUserName(User.Identity.Name);

                // Populate image, load users image if needed
                // userService does not set or load it, simply does not
                user.ImageUrl = _blobService.GetUserImagUrl(user);
                user.ImageUrlSmall = user.ImageUrlSmall;

                model = user.ToDto();
                model.IsSignedIn = true;
            }
            else
            {
                model = new ApplicationUserDto();
            }

            return View("CurrentUser", await Task.FromResult(model));
        }
    }
}
