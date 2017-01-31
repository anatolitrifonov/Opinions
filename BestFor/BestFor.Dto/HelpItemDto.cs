using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BestFor.Dto
{
    /// <summary>
    /// Represents the item from the help page
    /// </summary>
    public class HelpItemDto: BaseDto
    {
        public string Key { get; set; }

        public string CultureName { get; set; }

        public string HelpTitle { get; set; }

        public string Value { get; set; }
    }
}
