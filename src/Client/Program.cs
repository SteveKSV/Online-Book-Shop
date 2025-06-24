using Client.Data;
using Client.Helpers;
using Client.Services;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;

var envType = Environment.GetEnvironmentVariable("ENV_TYPE");
var isDocker = envType == "Docker";

var builder = WebApplication.CreateBuilder(args);

if (isDocker)
{
    builder.Configuration.AddJsonFile("appsettings.docker.json", optional: false, reloadOnChange: true);
}

string? connectionString;
if (isDocker)
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "YourDefaultPassword";

    connectionString = $"Server={dbHost};Database={dbName};User Id=sa;Password={dbPassword};TrustServerCertificate=True;Encrypt=False;";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ProtectedLocalStorage>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => 
    provider.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRatingService, RatingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();
