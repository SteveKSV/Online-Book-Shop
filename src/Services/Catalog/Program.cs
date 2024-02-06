using Catalog;
using Catalog.Managers.Interfaces;
using Catalog.Managers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddTransient<MongoDbContext>();

//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////
builder.Services.AddScoped<IBookManager, BookManager>();

//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
}


app.UseAuthorization();

app.MapControllers();

app.Run();
