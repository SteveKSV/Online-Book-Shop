using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<CardPayment> CardPayments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
                entity.Property(e => e.UpdatedAt).ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EmailAddress).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(100);

                // Foreign key to OrderStatus
                entity.HasOne(e => e.Status)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(e => e.StatusId);

                // One-to-one with Payment
                entity.HasOne(e => e.Payment)
                      .WithOne(p => p.Order)
                      .HasForeignKey<Payment>(p => p.OrderId);

                entity.HasMany(e => e.Items)
                      .WithOne(e => e.Order)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookId).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.HasOne(e => e.Book)
                  .WithMany(o => o.OrderItems)
                  .HasForeignKey(e => e.BookId)
                  .IsRequired();
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();

                entity.HasOne(p => p.PaymentMethod)
                      .WithMany(pm => pm.Payments)
                      .HasForeignKey(p => p.PaymentMethodId);
            });

            modelBuilder.Entity<CardPayment>(entity => 
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Payment)
                    .WithMany(p => p.CardPayments)
                    .HasForeignKey(cp => cp.PaymentId);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.Title)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(b => b.Authors)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasMany(b => b.OrderItems)
                      .WithOne(oi => oi.Book)
                      .HasForeignKey(oi => oi.BookId);
            });
        }
    }
}