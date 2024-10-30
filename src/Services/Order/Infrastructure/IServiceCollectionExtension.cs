using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class IServiceCollectionExtension
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext(configuration);
            services.AddRepositories();
        }

        private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Settings 
            var dbType = Environment.GetEnvironmentVariable("DB_TYPE");

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");

            var connectionString = $"Data Source={dbHost},1433;Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=True";
            
            connectionString = dbType == "Docker"
                ? connectionString
                : configuration.GetConnectionString("OrderConnectionString");

            services.AddDbContext<OrderContext>(options =>
               options.UseSqlServer(connectionString));

            var serviceProvider = services.BuildServiceProvider();
            
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
            context.Database.EnsureCreated();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
