using Discount.Grpc.Repositories;
using Discount.Grpc.Repositories.Interfaces;
using Discount.Grpc.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

//////////////////////// DATABASE CONFIGURATION ///////////////////////////////
builder.Services.AddScoped((s) => new SqlConnection(builder.Configuration.GetConnectionString("DapperConnection")));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly()); 
builder.Services.AddScoped<IDbTransaction>(s =>
{
    SqlConnection conn = s.GetRequiredService<SqlConnection>();
    conn.Open();
    return conn.BeginTransaction();
});

//////////////////////// MANAGER CONFIGURATION ///////////////////////////////
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
var app = builder.Build();



//////////////////////// gRPC CONFIGURATION ///////////////////////////////
app.MapGrpcService<DiscountService>();

app.MapGet("/", () => "Hello, Discount gRPC Service.");
app.Run();
