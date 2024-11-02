using IdentityModel;
using IdentityServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(opt =>
                opt.UseSqlServer(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Registering PersistedGrantDbContext
            services.AddOperationalDbContext(opt =>
            {
                opt.ConfigureDbContext = db =>
                    db.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            // Registering ConfigurationDbContext
            services.AddConfigurationDbContext(opt =>
            {
                opt.ConfigureDbContext = db =>
                    db.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            // Migrate PersistedGrantDbContext
            var persistedGrantDbContext = scope.ServiceProvider.GetService<PersistedGrantDbContext>();
            if (persistedGrantDbContext != null)
            {
                persistedGrantDbContext.Database.Migrate();
            }

            // Migrate ConfigurationDbContext
            var configDbContext = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            if (configDbContext != null)
            {
                configDbContext.Database.Migrate();
                EnsureSeedData(configDbContext);
            }

            // Migrate AspNetIdentityDbContext
            var aspNetIdentityDbContext = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            if (aspNetIdentityDbContext != null)
            {
                aspNetIdentityDbContext.Database.Migrate();
                EnsureRoles(scope); // Ensure roles are created
                EnsureUsers(scope); // Ensure users are created
            }
        }

        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var stepan = userMgr.FindByNameAsync("stepan").Result;
            if (stepan == null)
            {
                stepan = new IdentityUser
                {
                    UserName = "stepan",
                    Email = "stepan@gmail.com",
                    EmailConfirmed = true
                };

                var result = userMgr.CreateAsync(stepan, "Pass123$").Result;

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                // Assign the "Admin" role to the user
                result = userMgr.AddToRoleAsync(stepan, "Admin").Result; // Ensure this matches the role name created

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                result = userMgr.AddClaimsAsync(
                    stepan,
                    new Claim[]
                    {
                new Claim(JwtClaimTypes.Name, "Stepan Klem"),
                new Claim(JwtClaimTypes.GivenName, "Stepan"),
                new Claim(JwtClaimTypes.FamilyName, "Klem"),
                new Claim("location", "Ukraine")
                    }
                ).Result;

                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to add claims: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // If user already exists, you might want to check if they have the Admin role
                var isInRole = userMgr.IsInRoleAsync(stepan, "Admin").Result;
                if (!isInRole)
                {
                    var result = userMgr.AddToRoleAsync(stepan, "Admin").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to assign role to existing user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }


        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }

        private static void EnsureRoles(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" }; // Keep the names consistent
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result; // Check if role exists

                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
                }
            }
        }
    }
}
