using Basket.Managers.Interfaces;
using Basket.Managers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Discount.Grpc;
using Basket.API.GrpcServices;
using MassTransit;

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
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
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

//////////////////////// DISCOUNT gRPC CONFIGURATION ///////////////////////////////
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(discountGrpcConnection);
});

builder.Services.AddScoped<DiscountGrpcService>();

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
