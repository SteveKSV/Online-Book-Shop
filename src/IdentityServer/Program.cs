using IdentityServer;
using IdentityServer.Data;
using IdentityServer.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

//////////////////////// SEED DATABASE WITH STATIC FILES (IdentityServer project -> dotnet run /seed) ///////////////////////////////
var seed = args.Contains("/seed");

if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

//////////////////////// ASSEMBLY AND CONNECTION STRING ///////////////////////////////
var assembly = typeof(Program).Assembly.GetName().Name;

// Get the DB_TYPE environment variable
var dbType = Environment.GetEnvironmentVariable("DB_TYPE") ?? "local"; // Default to 'local'

// Set up connection string based on DB_TYPE
string connectionString;
if (dbType == "Docker")
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"; // Default to localhost
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "IdentityServerDb";
    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "YourDefaultPassword"; // Ensure a default or safe handling

    connectionString = $"Server={dbHost};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True";

    // Always seed data when using Docker
    SeedData.EnsureSeedData(connectionString!);
}
else
{
    connectionString = builder.Configuration.GetConnectionString("BookOnlineConnStr");

    // Seed data only if /seed is specified in arguments
    if (seed)
    {
        SeedData.EnsureSeedData(connectionString!);
    }
}

//////////////////////// DB CONTEXTS CONFIGURATION ///////////////////////////////
builder.Services.AddDbContext<AspNetIdentityDbContext>(opt =>
                opt.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AspNetIdentityDbContext>();

builder.Services.AddTransient<IProfileService, ProfileService>();

//////////////////////// IDENTITY-SERVER-4 CONFIGURATION ///////////////////////////////
builder.Services.AddIdentityServer()
        .AddProfileService<ProfileService>()
        .AddAspNetIdentity<IdentityUser>()
        .AddConfigurationStore(opt =>
        {
            opt.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString,
                            o => o.MigrationsAssembly(assembly));
        })
        .AddOperationalStore(opt =>
        {
            opt.ConfigureDbContext = b =>
                    b.UseSqlServer(connectionString,
                            o => o.MigrationsAssembly(assembly));
        })
        .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
