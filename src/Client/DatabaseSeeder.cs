using Client.Data;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;
using System.Globalization;

namespace Client
{
    public class UserCsvModel
    {
        public string Id { get; set; }
        public string UserName { get; set; } 
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
    }

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

            // Seed Admin User
            var adminEmail = "admin@example.com";
            var userEmail = "user@example.com";
            var adminPassword = "Admin@123";
            var userPassword = "User@123";

            var adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var user = new IdentityUser { UserName = userEmail, Email = userEmail };

            // Check if the admin user already exists by email
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

            // Check if any users already exist
            var existingUsers = await _userManager.Users.ToListAsync(); // Get all users from the database
            if (!existingUsers.Any())
            {
                // Seed Users from CSV
                string usersCsvPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "users.csv");
                var newUsers = ReadUsersFromCsv(usersCsvPath);

                foreach (var userCSV in newUsers)
                {
                    // Create user
                    var newUser = new IdentityUser
                    {
                        Id = userCSV.Id,
                        UserName = userCSV.UserName,
                        Email = userCSV.Email,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(newUser, userPassword);

                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "User");
                    }
                    else
                    {
                        // Log or handle failure
                        Console.WriteLine($"Failed to create user {userCSV.UserName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    }
                }

                Console.WriteLine($"Seeded {newUsers.Count} new users successfully.");
            }
            else
            {
                Console.WriteLine("Users already exist in the database. Skipping user seeding.");
            }
        }

        private List<UserCsvModel> ReadUsersFromCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Map CSV columns to `UserCsvModel` properties
            return csv.GetRecords<UserCsvModel>().ToList();
        }
    }
}

