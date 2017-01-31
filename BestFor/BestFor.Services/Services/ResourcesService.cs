using BestFor.Data;
using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Services.Cache;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BestFor.Services.Services
{
    /// <summary>
    /// Simple service that returns language specific strings by key.
    /// </summary>
    /// <remarks>
    /// Stores data in cache as list. For now we think we can get away with list because we are not planning to have more than 5k strings.
    /// We think we can go through this using linq just fine.
    /// Assumes that an instance is created per request and culture for a given instance is always the same.
    /// </remarks>
    public class ResourcesService : IResourcesService
    {
        public class CommonStrings
        {
            public CommonStringsDto Strings;

            public Dictionary<string, string> Translations;
        }
        /// <summary>
        /// Store services for between the calls.
        /// </summary>
        private ICacheManager _cacheManager;
        private IRepository<ResourceString> _repository;
        public const string DEFAULT_CULTURE = "en-US";
        /// <summary>
        /// Save data between calls. This object might be needed several times. No need to go to cache every time.
        /// </summary>
        private CommonStrings _commonStrings;

        public ResourcesService(ICacheManager cacheManager, IRepository<ResourceString> repository)
        {
            _cacheManager = cacheManager;
            _repository = repository;
        }

        /// <summary>
        /// Return common strings for a given culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <remarks>Has to be a function. I'd rather not set the culture per instance.</remarks>
        public CommonStringsDto GetCommonStrings(string culture)
        {
            // See if we got the strings already
            if (_commonStrings != null) return _commonStrings.Strings;
            // Well ... nothing we can do ... load from cache.
            var resourceStrings = GetCachedData();
            // Setup common strings so that we do not have to touch cache with no need.
            _commonStrings = LoadCommonStrings(culture, resourceStrings);
            return _commonStrings.Strings;
        }

        /// <summary>
        /// Get the string for a given key in a given culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>
        /// If string for culture found return.
        /// If not found seach English
        /// If not found return key
        /// </remarks>
        public string GetString(string culture, string key)
        {
            // Do some checks before we go to cache.
            // No need to touch cache if blanks.
            if (string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture)) return key;
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) return key;
            // Ok get cached strings
            var resourceStrings = GetCachedData();
            // Setup common strings so that we do not have to touch cache with no need.
            if (_commonStrings == null) _commonStrings = LoadCommonStrings(culture, resourceStrings);
            // Get the string for this culture
            string result = FindOneString(culture, key, resourceStrings);
            result = ReplacePatterns(result, _commonStrings.Translations);
            return result;
        }

        /// <summary>
        /// Find a set of strings for set of keys
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public string[] GetStrings(string culture, string[] keys)
        {
            // Do some checks before we go to cache.
            // No need to touch cache if blanks.
            if (string.IsNullOrEmpty(culture) || string.IsNullOrWhiteSpace(culture)) return keys;
            if (keys == null) return keys;
            var result = new string[keys.Length];
            // Ok get cached strings
            var resourceStrings = GetCachedData();
            // Setup common strings so that we do not have to touch cache with no need.
            if (_commonStrings == null) _commonStrings = LoadCommonStrings(culture, resourceStrings);
            // Loop through the resources 
            for (var i = 0; i < keys.Length; i++)
            {
                if (string.IsNullOrEmpty(keys[i]) || string.IsNullOrWhiteSpace(keys[i]))
                    result[i] = keys[i];
                else
                    result[i] = FindOneString(culture, keys[i], resourceStrings);
                result[i] = ReplacePatterns(result[i], _commonStrings.Translations);
            }
            return result;
        }

        /// <summary>
        /// Find strings for keys and return as javascript json object.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="javaScriptVariableName"></param>
        /// <param name="keys"></param>
        /// <returns>
        /// script
        /// var javaScriptVariableName {
        /// "a" : "s",
        /// "b" : "v"
        /// }
        /// script
        /// </returns>
        public string GetStringsAsJavaScript(string culture, string javaScriptVariableName, string[] keys)
        {
            if (keys == null) return null;
            var strings = GetStrings(culture, keys);
            var sb = new StringBuilder("<script>\n\r")
                .Append("var ").Append(javaScriptVariableName).Append(" = {\n\r");
            for (var i = 0; i < keys.Length; i++)
            {
                // We need to check for doublequotes in values
                sb.Append("\"").Append(keys[i]).Append("\" : \"").Append(strings[i]).Append(i < keys.Length - 1 ? "\",\n\r" : "\"\n\r");
            }
            sb.Append("}\r\n</script>\n\r");
            return sb.ToString();
        }

        /// <summary>
        /// Return dynamic json object containing keys and strings as properties and values.
        /// This object can be then rendered as jSon object on the client side.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public JObject GetStringsAsJson(string culture, string[] keys)
        {
            if (keys == null) return null;

            // Get strings -> build json object
            var strings = GetStrings(culture, keys);
            var result = new JObject();
            for (var i = 0; i < keys.Length; i++)
                result.Add(new JProperty(keys[i], strings[i]));
            return result;
        }

        /// <summary>
        /// Load all known common strings for all cultures.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, CommonStringsDto> GetCommonStringsForAllCultures()
        {
            var data = GetCachedData().Where(x => x.Key == "best_start_capital" || x.Key == "for_lower" || x.Key == "is_lower").ToList();
            var result = new Dictionary<string, CommonStringsDto>();
            var cultures = data.Select(x => x.CultureName).Distinct();
            foreach (var culture in cultures)
                result.Add(culture, LoadCommonStrings(culture, data).Strings);
            return result;
        }

        /// <summary>
        /// Find a string by key and culture in the list of resource strings
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="resourceStrings"></param>
        /// <returns></returns>
        public string FindOneString(string culture, string key, List<ResourceString> resourceStrings)
        {
            var getStringResult = resourceStrings.FirstOrDefault(x => x.CultureName == culture && x.Key == key);
            if (getStringResult != null) return getStringResult.Value;
            // Not found, get default
            getStringResult = resourceStrings.FirstOrDefault(x => x.CultureName == DEFAULT_CULTURE && x.Key == key);
            if (getStringResult != null) return getStringResult.Value;
            // Not found, return key
            return key;
        }

        /// <summary>
        /// Loop through all common strings and replace the pattern.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="commonStrings"></param>
        /// <returns></returns>
        public string ReplacePatterns(string input, Dictionary<string, string> commonStringTranslations)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) return input;
            // no patterens -> exit
            if (input.IndexOf("{{") < 0) return input;
            foreach (string key in commonStringTranslations.Keys)
                input = input.Replace("{{" + key + "}}", commonStringTranslations[key]);
            return input;
        }

        #region Private Methods
        /// <summary>
        /// Get data from cache. Load into cache from repo if needed.
        /// </summary>
        /// <returns></returns>
        private List<ResourceString> GetCachedData()
        {
            object data = _cacheManager.Get(CacheConstants.CACHE_KEY_RESOURCES_DATA);
            if (data == null)
            {
                // Load all.
                List<ResourceString> resourceStrings = _repository.List().ToList();
                // Save
                _cacheManager.Add(CacheConstants.CACHE_KEY_RESOURCES_DATA, resourceStrings);
                return resourceStrings;
            }
            return (List<ResourceString>)data;
        }

        /// <summary>
        /// Load common strings for the current culture.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="resourceStrings"></param>
        /// <returns></returns>
        private CommonStrings LoadCommonStrings(string culture, List<ResourceString> resourceStrings)
        {
            var result = new CommonStrings();
            //TODO 
            result.Strings = new CommonStringsDto();
            //TODO
            result.Translations = new Dictionary<string, string>();

            result.Strings.Best = LoadOneCommonString(culture, "best_start_capital", resourceStrings, result.Translations);
            result.Strings.For = LoadOneCommonString(culture, "for_lower", resourceStrings, result.Translations);
            result.Strings.Is = LoadOneCommonString(culture, "is_lower", resourceStrings, result.Translations);
            result.Strings.FlagLower = LoadOneCommonString(culture, "flag_lower", resourceStrings, result.Translations);
            result.Strings.FlagUpper = LoadOneCommonString(culture, "flag_upper", resourceStrings, result.Translations);
            result.Strings.VoteLower = LoadOneCommonString(culture, "vote_lower", resourceStrings, result.Translations);
            result.Strings.VoteUpper = LoadOneCommonString(culture, "vote_upper", resourceStrings, result.Translations);
            result.Strings.DescribeLower = LoadOneCommonString(culture, "describe_lower", resourceStrings, result.Translations);
            result.Strings.DescribeUpper = LoadOneCommonString(culture, "describe_upper", resourceStrings, result.Translations);
            result.Strings.MoreLower = LoadOneCommonString(culture, "more_lower", resourceStrings, result.Translations);
            result.Strings.MoreUpper = LoadOneCommonString(culture, "more_upper", resourceStrings, result.Translations);
            result.Strings.PostersUpper = LoadOneCommonString(culture, "posters_capital", resourceStrings, result.Translations);
            result.Strings.GuestUpper = LoadOneCommonString(culture, "guest_capital", resourceStrings, result.Translations);
            return result;
        }

        /// <summary>
        /// Given a resource string key find it in resource strings for a given culture.
        /// Also add it to translations dictionary.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="key"></param>
        /// <param name="resourceStrings"></param>
        /// <param name="translations"></param>
        /// <returns></returns>
        private string LoadOneCommonString(string culture, string key, List<ResourceString> resourceStrings,
            Dictionary<string, string> translations)
        {
            var result = FindOneString(culture, key, resourceStrings);
            translations.Add(key, result);
            return result;
        }
        #endregion
    }
}
