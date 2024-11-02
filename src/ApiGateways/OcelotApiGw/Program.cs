using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder();
//builder.Services.AddAuthentication()
//    .AddJwtBearer("IdentityApiKey", x =>
//    {
//        x.Authority = "https://localhost:5006"; // IDENTITY SERVER URL
//        x.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateAudience = false
//        };
//    });

var envVariable = Environment.GetEnvironmentVariable("ENVIRONMENT");
if (envVariable == "Docker")
{
    builder.Configuration.AddJsonFile("ocelot.docker.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
}

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

app.UseRouting();

await app.UseOcelot();

app.Run();

