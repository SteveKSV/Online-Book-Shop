using Microsoft.OpenApi.Models;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

//////////////////////// CLASS LIBRARIES CONFIGURATION ///////////////////////////////
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceLayer(builder.Configuration);

//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

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
