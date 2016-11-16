using BestFor.Dto.Account;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        private IAnswerService _answerService;

        public TopPostersViewComponent(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        /// <summary>
        /// This is the main rendering method
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ApplicationUsersDto();
            model.Users = await _answerService.FindTopPosterIds();
            return View(model);
        }
    }
}
