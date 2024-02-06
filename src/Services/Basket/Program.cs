using Basket.Managers.Interfaces;
using Basket.Managers;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
builder.Services.AddScoped<IBasketManager, BasketManager>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
