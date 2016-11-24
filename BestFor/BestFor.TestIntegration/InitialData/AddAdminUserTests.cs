using BestFor.Data;
using BestFor.Domain;
using BestFor.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BestFor.UnitTests.Data
{
    [ExcludeFromCodeCoverage]
    public class AddAdminUserTests
    {
        /// <summary>
        /// This test will add admin roles and user if they are missing in the database.
        /// </summary>
        /// <returns></returns>
        public async Task AddAdminUserTests_AddAdminUser_AddsAdminUser()
        {
            // Uncomment this to actually run.
            var t = 5; if (t > 1) return;
            // Result of some operations
            IdentityResult identityResult = null;

            // Create data context
            var dataContext = new BestDataContext();

            // Create role manager
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dataContext), null, null, null, null, null);
            // Search for admin role.
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin.ToString());
            // Create role if does not exist.
            if (adminRole == null)
            {
                adminRole = new IdentityRole(Roles.Admin.ToString());
                identityResult = await roleManager.CreateAsync(adminRole);
                if (!identityResult.Succeeded)
                    throw new Exception("Could not create role");
            }

            // Create password hasher
            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            // Create userManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dataContext), null, passwordHasher, null, null, null, null, null, null);
            // Search for admin user
            var adminUser = await userManager.FindByEmailAsync("admin@bestfor.com");
            // Create user if does not exist.
            if (adminUser == null)
            {
                adminUser = new ApplicationUser();
                adminUser.Email = "admin@bestfor.com";
                // adminUser.NormalizedEmail = adminUser.Email.ToUpper();
                adminUser.UserName = "admin@bestfor.com";
                // adminUser.NormalizedUserName = adminUser.Email.ToUpper();
                identityResult = await userManager.CreateAsync(adminUser, "1zzzelllo_Boom");
                if (!identityResult.Succeeded)
                    throw new Exception("Could add admin user");
            }

            // Add user to role or role to user. Variable is for debugging.
            var isInRole = await userManager.IsInRoleAsync(adminUser, Roles.Admin.ToString());
            // Add if not there
            if (!isInRole)
            {
                identityResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin.ToString());
                if (!identityResult.Succeeded)
                    throw new Exception("Could not add person to role");
            }

        }
    }
}
