namespace BestFor.Services
{
    /// <summary>
    /// Helps clean up user input in case it is pasted from Word and such editors.
    /// </summary>
    public class TextCleaner
    {
        public static string Clean(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) return input;

            var result = input.Replace((char)147, '"').Replace((char)148, '"'); // word's double quotes starting and ending
            result = result.Replace((char)8220, '"').Replace((char)8221, '"'); // word's double quotes starting and ending

            result = result.Replace(((char)133).ToString(), "..."); // ...
            result = result.Replace(((char)8230).ToString(), "..."); // ...

            result = result.Replace((char)146, '\''); // single quote.
            result = result.Replace((char)8217, '\''); // single quote.
            result = result.Replace((char)145, '\''); // single quote.
            result = result.Replace((char)8216, '\''); // single quote.

            result = result.Replace((char)8211, '-'); // single quote.

            return result;
        }
    }
}
