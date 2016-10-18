using BestFor.Services.Profanity;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    public interface IProfanityService
    {
        ProfanityCheckResult CheckProfanity(string input);

        /// <summary>
        /// Error message will be localized for the passed culture
        /// </summary>
        /// <param name="input"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        ProfanityCheckResult CheckProfanity(string input, string culture);
    }
}
