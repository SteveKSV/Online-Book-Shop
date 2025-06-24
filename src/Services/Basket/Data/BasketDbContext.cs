using Basket.Entities;
using Microsoft.EntityFrameworkCore;

namespace Basket.Data
{
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

        public DbSet<BasketItem> BasketItems { get; set; }
    }
}
