using Microsoft.OpenApi.Models;
using Application;
using Infrastructure;
using MassTransit;
using EventBusMessages.Common;
using Order;

var builder = WebApplication.CreateBuilder(args);

//////////////////////// CLASS LIBRARIES CONFIGURATION ///////////////////////////////

builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// MASSTRANSIT CONFIGURATION ///////////////////////////////
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration.GetValue<string>("EventBusSettings:HostAddress")));
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(context);
        });
    });
});

//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order.API v1"));
}

app.UseAuthorization();
app.MapControllers();
app.Run();
