using Shopping.Aggregator.Services.Interfaces;
using Shopping.Aggregator.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
 c.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSettings:CatalogUrl").Value!));
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
 c.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSettings:BasketUrl").Value!));
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
 c.BaseAddress = new Uri(builder.Configuration.GetSection("ApiSettings:OrderingUrl").Value!));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shopping.Aggregator", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping.Aggregator v1"));
}

app.UseAuthorization();

app.MapControllers();
app.UseHttpsRedirection();
app.Run();
