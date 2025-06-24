using Catalog.Managers.Interfaces;
using Catalog.Managers;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WhatToReadConnStr")));
//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////
builder.Services.AddScoped<IBookManager, BookManager>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisUrl = builder.Configuration["RedisUrl"] ?? "localhost:6379";

    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    {
        EndPoints = { redisUrl },
        AbortOnConnectFail = false,        // Не кидати виняток, якщо Redis недоступний при старті
        ConnectTimeout = 100,              // Швидкий фейл при неможливості з'єднання (мс)
        SyncTimeout = 100,                 // Таймаут синхронних операцій (мс)
        KeepAlive = 10,                    // Підтримка з'єднання
        ConnectRetry = 1,                  // Кількість повторів при фейлі з'єднання
        DefaultDatabase = 0,
        AllowAdmin = false
    };

    options.InstanceName = "Catalog_";
});


builder.Services.AddScoped<IGenreManager, GenreManager>();
builder.Services.AddScoped<ICommentManager, CommentManager>();
builder.Services.AddScoped<IRatingManager, RatingManager>();
builder.Services.AddScoped<IRecommendationManager, RecommendationManager>();

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
app.Run();
