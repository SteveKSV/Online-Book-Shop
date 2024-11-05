using Client.Data;
using Microsoft.AspNetCore.Identity;

namespace Client
{
    public class DatabaseSeeder
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Seed Roles
            string[] roleNames = { "Administrator", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Users
            var adminEmail = "admin@example.com";
            var userEmail = "user@example.com";
            var adminPassword = "Admin@123";
            var userPassword = "User@123";

            // Updated admin user details
            var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail }; // Updated username
            var user = new IdentityUser { UserName = userEmail, Email = userEmail };

            // Seed Admin User
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var createResult = await _userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    // Confirm the admin user
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(adminUser);
                    await _userManager.ConfirmEmailAsync(adminUser, token);
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed Regular User
            if (await _userManager.FindByEmailAsync(userEmail) == null)
            {
                var createResult = await _userManager.CreateAsync(user, userPassword);
                if (createResult.Succeeded)
                {
                    // Confirm the regular user
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, token);
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}
