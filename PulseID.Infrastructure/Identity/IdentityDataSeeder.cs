using Microsoft.AspNetCore.Identity;
using PulseID.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseID.Infrastructure.Identity
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(AppRoles.SuperAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(AppRoles.SuperAdmin));
            }


            var adminEmail = "SuperAdmin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "SuperAdmin#1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.SuperAdmin);
                }
            }
            else
            { 
                if (!await userManager.IsInRoleAsync(adminUser, AppRoles.SuperAdmin))
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.SuperAdmin);
                }
            }
        }
    }
}
