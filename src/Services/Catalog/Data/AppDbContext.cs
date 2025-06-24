using Catalog.Entities;
using DnsClient;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Price)
                  .HasColumnType("decimal(10, 2)");

            entity.HasOne(x => x.Order)
                  .WithMany(x => x.Items)
                  .HasForeignKey(x => x.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(e => e.CommentText).HasColumnName("Comment");

            entity.HasOne(c => c.Book)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasMany(x => x.Comments)
                .WithOne(x => x.Book)
                .HasForeignKey(x => x.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasMany(x => x.Books)
                .WithOne(x => x.Genre)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(r => new { r.UserId, r.BookId }).IsUnique();

            entity.Property(r => r.Value)
                .IsRequired()
                .HasColumnName("Rating");

            entity.Property(r => r.RatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(r => r.Book)
                  .WithMany()
                  .HasForeignKey(r => r.BookId);
        });
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}
