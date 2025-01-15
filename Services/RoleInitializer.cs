using Microsoft.AspNetCore.Identity;
using Onboarding.Models;

namespace Onboarding.Services
{
    public static class RoleInitializer
    {
        public static async System.Threading.Tasks.Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var roles = new List<string>
            {
                "Admin",
                "Buddy",
                "Mentor",
                "Manager",
                "Nowy",
                "HR",
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            //konto HR
            var hrUser = await userManager.FindByEmailAsync("hr@mail.com");
            if (hrUser == null)
            {
                var newUser = new User
                {
                    UserName = "hr@mail.com",
                    Email = "hr@mail.com",
                    Name = "HR",
                    Surname = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser, "HrPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "HR");
                }
            }

            //konto Admina
            var adminUser = await userManager.FindByEmailAsync("admin@mail.com");
            if (adminUser == null)
            {
                var newUser = new User
                {
                    UserName = "admin@mail.com",
                    Email = "admin@mail.com",
                    Name = "Admin",
                    Surname = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newUser, "AdminPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                }
            }

        }
    }
}
