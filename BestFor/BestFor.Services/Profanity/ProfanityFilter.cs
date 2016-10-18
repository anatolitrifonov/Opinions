using System.Collections.Generic;
using System.Text.RegularExpressions;
using BestFor.Domain.Entities;

namespace BestFor.Services.Profanity
{
    public class ProfanityFilter
    {
        /// <summary>
        /// Checks if line has unprintable characters.
        /// Linebreaks are allowed.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool AllCharactersAllowed(string data)
        {
            // \x2022 is a unicode bullet point
            Regex r = new Regex("[^\x20-\x7e\r\n\t\x2022\xa1-\x10fffd]");
            // var matches = r.Matches(data);
            return !r.IsMatch(data);
        }

        public static string FirstDisallowedCharacter(string data)
        {
            // \x2022 is a unicode bullet point
            Regex r = new Regex("[^\x20-\x7e\r\n\t\x2022\xa1-\x10fffd]");
            var matches = r.Matches(data);
            if (matches.Count > 0)
                return matches[0].Value;
            return null;
        }

        public static string CleanupData(string input)
        {
            // first level cleanup. Change C0m to com. D!nk to dink.
            // TODO speed this up
            //var result = input;
            //foreach(var key in _data.Keys)
            //    result = Regex.Replace(result, _data[key], key);
            //return result;
            if (string.IsNullOrEmpty(input)) return input;
            if (string.IsNullOrWhiteSpace(input)) return input;
            // For now just to lower. Regular expressions still do not work
            return input.ToLower();
        }

        public static string GetProfanity(string input, IEnumerable<BadWord> badwords)
        {
            var localInput = CleanupData(input);
            if (localInput == null) return null;
            foreach (var word in badwords)
            {
                if (localInput.EndsWith(" " + word.Phrase)) return word.Phrase;
                if (localInput.EndsWith(word.Phrase + ".")) return word.Phrase;
                if (localInput.StartsWith(word.Phrase + " ")) return word.Phrase;
                if (localInput.Contains(" " + word.Phrase + " ")) return word.Phrase;
                if (localInput == word.Phrase) return word.Phrase;

                var result = CheckContains(localInput, word.Phrase, " `~!@#$%^&*()-_+=[{]};:'\"<>/?,.№");
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Checks is input string contains a bad word surrounded by any funny characters
        /// Example "$fuck$" or "!fuck!" or " fuck!" will be all flagged if bad word is fuck
        /// and charsToCheck is " !"
        /// </summary>
        /// <param name="input"></param>
        /// <param name="badWord"></param>
        /// <param name="charsToCheck"></param>
        /// <returns></returns>
        public static string CheckContains(string input, string badWord, string charsToCheck)
        {
            var chars = charsToCheck.ToCharArray();
            foreach (var c in chars)
                foreach (var c1 in chars)
                {
                    var check = c + badWord + c1;
                    if (input.Contains(check))
                        return c + badWord + c1;
                }
            return null;
        }
    }
}
