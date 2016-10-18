using System.Threading;
using Microsoft.AspNetCore.Mvc;
using BestFor.Dto;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace BestFor.Controllers
{
    /// <summary>
    /// ResponseCache applies to all controllers if I am not mistaken.
    /// </summary>
    [ResponseCache(CacheProfileName = "Hello")]
    public abstract class BaseApiController : Controller
    {
        /// <summary>
        /// Pages working with these controllers are expected to pass the header with this name.
        /// </summary>
        public const string ANTI_FORGERY_HEADER_NAME = "ANTI_FORGERY_HEADER";
        /// <summary>
        /// Sometimes pages will send this cookie.
        /// </summary>
        public const string ANTI_FORGERY_COOKIE_NAME = "ANTI_FORGERY_COOKIE_NAME";
        public const string DEBUG_REACTJS_URL_PARAMETER_NAME = "debugreact";

        public const string DEFAULT_CULTURE = "en-US";

        /// <summary>
        /// URL may contain referene to cuture as /culture/controller/action/something
        /// This class will parse the culture and store it between calls to save on parsing.
        /// Unfortunately url is parsed twice: one time in routing contraint and another one here if controller asks for culture value.
        /// </summary>
        private string _culture = null;
        public string Culture
        {
            get
            {
                
                return _culture ?? ParseCulture();
            }
        }

        /// <summary>
        /// Cut the culture in request path
        /// </summary>
        protected string RequestPathNoCulture
        {
            get
            {
                var requestPath = Request.Path.Value;
                // cut the culture
                var cultureBegining = "/" + Culture.ToLower();
                if (requestPath.ToLower().StartsWith(cultureBegining)) requestPath = requestPath.Substring(cultureBegining.Length);
                return requestPath;
            }
        }

        protected bool ParseAntiForgeryHeader(IAntiforgery antiforgery, CrudMessagesDto dto, HttpContext httpContext)
        {
            var task = antiforgery.IsRequestValidAsync(httpContext);
            task.Wait();
            var validRequest = task.Result;
            if (!validRequest)
            {
                dto.ErrorMessage = "Broken antiforgery";
                return false;
            }
            return true;
        }
        /// <summary>
        /// Read Url parameter as string
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected string ReadUrlParameter(string parameterName)
        {
            if (!Request.Query.ContainsKey(parameterName)) return null;
            return Request.Query[parameterName][0];
        }

        /// <summary>
        /// Read Url parameter as bool
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected bool ReadUrlParameterAsBoolean(string parameterName)
        {
            bool result;
            if (bool.TryParse(ReadUrlParameter(parameterName), out result)) return result;
            return false;
        }

        #region Private Methods
        /// <summary>
        /// Parse the culture string from the url or return the default culture string
        /// </summary>
        /// <returns></returns>
        private string ParseCulture()
        {
            _culture = DEFAULT_CULTURE;
            var requestPath = Request.Path.ToString();
            // theoretically this can never happen
            if (string.IsNullOrEmpty(requestPath) || string.IsNullOrWhiteSpace(requestPath)) return SetCulture(_culture);
            // Theoretically requestPath can never start with space because spaces in URL are replaces with %20 but we will check anyway
            requestPath = requestPath.Trim();
            if (requestPath == "/") return SetCulture(_culture);
            // URL "/////blah" is converted by browser to "/blah". Therefor we can never get into the situation of path being "///blah".
            // Although I only checked Chrome
            // first remove the starting "/"
            if (requestPath.StartsWith("/")) requestPath = requestPath.Substring(1);
            // We do not need split since it is expensive. We only need the first word.
            // Reminder that it can't be // or blank so condition is 
            var culture = requestPath.IndexOf("/") > 0 ? requestPath.Substring(0, requestPath.IndexOf("/")).ToLower() : requestPath.ToLower();
            // We can also try to trim the culture but this would be rather extreme.
            // Got the culture ... now let's see if we know it.
            if (culture == "fi" || culture == "fi-fi") return SetCulture("fi-FI");
            if (culture == "ru" || culture == "ru-ru") return SetCulture("ru-RU");
            return SetCulture(DEFAULT_CULTURE);
        }

        /// <summary>
        /// Two lines shortcut to set the variable and return it.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private string SetCulture(string culture)
        {
            _culture = culture;
            // Save the culture in the thread.
            Thread.SetData(Thread.GetNamedDataSlot("SavedThreadCulture"), culture);
            return _culture;
        }
        #endregion
    }
}
