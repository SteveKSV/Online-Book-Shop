using Catalog;
using Catalog.Managers.Interfaces;
using Catalog.Managers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Catalog.Data;

var builder = WebApplication.CreateBuilder(args);

//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
var dbType = Environment.GetEnvironmentVariable("DB_TYPE");
var connectionString = dbType == "Docker"
    ? $"mongodb://{Environment.GetEnvironmentVariable("DB_HOST")}:27017/CatalogDb"     // Docker connection string 
    : builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value; // Local connection string

builder.Services.Configure<DatabaseSetting>(settings =>
{
    settings.ConnectionString = connectionString;
    settings.DatabaseName = Environment.GetEnvironmentVariable("DB_NAME") ?? "CatalogDb";
});
builder.Services.AddTransient<MongoDbContext>();

//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////
builder.Services.AddScoped<IBookManager, BookManager>();
builder.Services.AddScoped<IGenreManager, GenreManager>();
builder.Services.AddScoped<ILanguageManager, LanguageManager>();
//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// AUTHENTICATION CONFIGURATION ///////////////////////////////
//builder.Services.AddAuthentication("Bearer")
//          .AddIdentityServerAuthentication("Bearer", options =>
//          {
//              options.Authority = "https://localhost:5006";
//              options.ApiName = "Catalog";
//          });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "Catalog.read");
    });
});

//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });

    // Security
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Seed the database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mongoDbContext = services.GetRequiredService<MongoDbContext>();
    CatalogContextSeed.SeedData(mongoDbContext.Books); // Call the seeding method here
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
