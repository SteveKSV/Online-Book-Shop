using Discount.Managers;
using Discount.Managers.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);


//////////////////////// CONTROLLERS CONFIGURATION ///////////////////////////////
builder.Services.AddControllers();

//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
builder.Services.AddScoped((s) => new SqlConnection(builder.Configuration.GetConnectionString("DapperConnection")));
builder.Services.AddScoped<IDbTransaction>(s =>
{
    SqlConnection conn = s.GetRequiredService<SqlConnection>();
    conn.Open();
    return conn.BeginTransaction();
});

//////////////////////// MANAGER CONFIGURATION ///////////////////////////////
builder.Services.AddScoped<IDiscountManager, DiscountManager>();

//////////////////////// SWAGGER CONFIGURATION ///////////////////////////////
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Discount.API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount.API v1"));
}

app.UseAuthorization();
app.MapControllers();
app.Run();
