﻿using BestFor.Dto;
using BestFor.Services.Profanity;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BestFor.Controllers
{
    [Route("api/[controller]")]
    public class SuggestionController : BaseApiController
    {
        /// <summary>
        /// Makes working with validation more comfortable
        /// </summary>
        private class ValidationResult
        {
            public bool Passed { get; set; } = true;

            public string ErrorMessage { get; set; }

            public string CleanedInput { get; set; }
        }

        private const string QUERY_STRING_PARAMETER_USER_INPUT = "userInput";
        private const int MINIMAL_WORD_LENGTH = 2;

        private ISuggestionService _suggestionService;
        private IProfanityService _profanityService;
        private IAntiforgery _antiforgery;

        public SuggestionController(ISuggestionService suggestionService, IProfanityService profanityService, IAntiforgery antiforgery)
        {
            _suggestionService = suggestionService;
            _profanityService = profanityService;
            _antiforgery = antiforgery;
        }

        // GET: api/values
        // Does not look like we can use this here [ValidateAntiForgeryToken]
        // see the code below. Request is valid but [ValidateAntiForgeryToken] throws it away
        [HttpGet]
        public SuggestionsDto Get()
        {
            var result = new SuggestionsDto();
            
            // check headers for antiforgery tokens
            if (!ParseAntiForgeryHeader(_antiforgery, result, HttpContext))
                return result;

            // validate input
            var validationResult = ValidateInputForGet();
            if (!validationResult.Passed)
            {
                result.ErrorMessage = validationResult.ErrorMessage;
                return result;
            }
            result.Suggestions = _suggestionService.FindSuggestions(validationResult.CleanedInput);

            return result;
        }

        /// <summary>
        /// Returns validated query string for GET method or null.
        /// </summary>
        /// <returns></returns>
        private ValidationResult ValidateInputForGet()
        {
            // do not return anything on empty input
            if (!Request.Query.ContainsKey(QUERY_STRING_PARAMETER_USER_INPUT)) return null;
            var userInput = Request.Query[QUERY_STRING_PARAMETER_USER_INPUT][0];
            // Check null or spaces or empty
            if (string.IsNullOrEmpty(userInput) || string.IsNullOrWhiteSpace(userInput)) return null;
            userInput = userInput.Trim();
            // Check minimal length
            if (userInput.Length < MINIMAL_WORD_LENGTH + 1) return null;
            // This is what we will return
            var result = new ValidationResult() { CleanedInput = userInput };
            // Check for bad characters.
            string badCharacter = ProfanityFilter.FirstDisallowedCharacter(result.CleanedInput);
            // let's only serve alphanumeric for now.
            if (badCharacter != null)
            {
                result.Passed = false;
                result.ErrorMessage = badCharacter;
            }

            return result;
        }
    }
}
