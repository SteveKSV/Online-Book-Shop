using Basket.Managers.Interfaces;
using Basket.Managers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using Basket.Data;

var builder = WebApplication.CreateBuilder(args);

//////////////////////// CONFIGURE ENVIRONMENT VARIABLES ///////////////////////////////
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// Use Docker environment variables if in Docker, otherwise default to appsettings.json values
var eventBusHost = environment == "Docker"
    ? Environment.GetEnvironmentVariable("EventBusSettings__HostAddress")
    : builder.Configuration.GetValue<string>("EventBusSettings:HostAddress");

var redisConnectionString = environment == "Docker"
    ? Environment.GetEnvironmentVariable("CacheSettings__ConnectionString")
    : builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");

var discountGrpcConnection = environment == "Docker"
    ? Environment.GetEnvironmentVariable("DiscountGrpcConnection__ConnectionString")
    : builder.Configuration.GetValue<string>("DiscountGrpcConnection:ConnectionString");

//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////
builder.Services.AddHttpClient<ICatalogService, CatalogService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5005"); 
});
builder.Services.AddScoped<IBasketManager, BasketManager>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//////////////////////// MANAGERS CONFIGURATION ///////////////////////////////

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(eventBusHost));
        cfg.ConfigureEndpoints(context);
    });
});

//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
builder.Services.AddDbContext<BasketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WhatToReadConnStr")));

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
