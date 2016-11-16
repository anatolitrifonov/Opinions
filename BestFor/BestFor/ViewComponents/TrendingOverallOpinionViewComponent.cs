using BestFor.Dto;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BestFor.ViewComponents
{
    /// <summary>
    /// Shows all top opinions.
    /// The view for this view component in Views/Shared/TrendingOverall
    /// </summary>
    public class TrendingOverallOpinionViewComponent : ViewComponent
    {
        /// <summary>
        /// Constructor injected answer service. Used for loading the answers.
        /// </summary>
        private IAnswerService _answerService;

        public TrendingOverallOpinionViewComponent(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        /// <summary>
        /// This is the main rendering method
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AnswersDto();
            model.Answers = await _answerService.FindAnswersTrendingOverall();
            return View(model);
        }
    }
}
