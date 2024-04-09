using IdentityServer;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

//////////////////////// SEED DATABASE WITH STATIC FILES (IdentityServer project -> dotnet run /seed) ///////////////////////////////
var seed = args.Contains("/seed");

if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

//////////////////////// ASSEMBLY AND CONNECTION STRING ///////////////////////////////
var assembly = typeof(Program).Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString("BookOnlineConnStr");

if (seed)
{
    SeedData.EnsureSeedData(connectionString!);
}

//////////////////////// DB CONTEXTS CONFIGURATION ///////////////////////////////
builder.Services.AddDbContext<AspNetIdentityDbContext>(opt =>
                opt.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AspNetIdentityDbContext>();

//////////////////////// IDENTITY-SERVER-4 CONFIGURATION ///////////////////////////////
builder.Services.AddIdentityServer()
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
