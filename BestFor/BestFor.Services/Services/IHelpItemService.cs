using BestFor.Dto;
using System.Collections.Generic;

namespace BestFor.Services.Services
{
    /// <summary>
    /// Interface to work with flags for answers and flags for answer descriptions
    /// </summary>
    public interface IHelpItemService
    {
        IEnumerable<HelpItemDto> FindAll(string culture);
    }
}
