using BestFor.Dto.Account;

namespace BestFor.Services.Services
{
    public interface IStatisticsService
    {
        void LoadUserStatictics(ApplicationUserDto user);
    }
}
