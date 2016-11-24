using BestFor.Data;
using BestFor.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BestFor.TestIntegration.Users
{
    [ExcludeFromCodeCoverage]
    public class UsersHelper
    {
        public async Task AddTwentyUsers()
        {
            // Uncomment this to actually run.
            var t = 5; if (t > 1) return;

            // Result of some operations
            IdentityResult identityResult = null;

            // Create data context
            var dataContext = new BestDataContext();

            // Create role manager
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dataContext), null, null, null, null, null);

            // Create password hasher
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            // Create userManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dataContext), null, passwordHasher, null, null, null, null, null, null);

            for (int i = 0; i < 20; i ++)
            {
                string userName = "testUser" + i;
                string userEmail = userName + "@apinioner.com";
                string userPassword = "cpe9-VZ234234";
                // Search for user
                var user = await userManager.FindByEmailAsync(userEmail);
                // Create user if does not exist.
                if (user == null)
                {
                    user = new ApplicationUser();
                    user.Email = userEmail;
                    user.UserName = userName;
                    identityResult = await userManager.CreateAsync(user, userPassword);
                }
            }
        }
    }
}
