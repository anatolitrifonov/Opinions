using BestFor.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.Services.Services
{
    public interface IStatisticsService
    {
        void LoadUserStatictics(ApplicationUser user);
    }
}
