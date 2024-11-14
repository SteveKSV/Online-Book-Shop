using Catalog;
using Catalog.Managers.Interfaces;
using Catalog.Managers;
using Microsoft.OpenApi.Models;

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
//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();


//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}

app.UseRouting();


app.MapControllers();

var mongoDbContext = app.Services.GetRequiredService<MongoDbContext>();
var database = mongoDbContext.Database;

// Paths to CSV files. Update paths for Docker volume or container resource folder.
var booksCsvPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "catalog.csv");
var warehouseBooksCsvPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "warehouse-books.csv");


await DatabaseInitializer.InitializeCollections(database, booksCsvPath, warehouseBooksCsvPath);


app.Run();
