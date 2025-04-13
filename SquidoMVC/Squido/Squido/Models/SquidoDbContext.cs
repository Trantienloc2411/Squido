using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using Squido.Models.Entities;

namespace Squido.Models;

public class SquidoDbContext : DbContext
{
    public SquidoDbContext(DbContextOptions<SquidoDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ImageBook> ImageBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define table names
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<Book>().ToTable("Books");
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<Rating>().ToTable("Ratings");
        modelBuilder.Entity<Order>().ToTable("Orders");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        modelBuilder.Entity<ImageBook>().ToTable("ImageBook");

        // Additional configurations can be added here
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne<User>(c => c.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>(o => o.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OrderItem>()
            .HasOne<Book>(o => o.Book)
            .WithMany(b => b.OrderItems)
            .HasForeignKey(o => o.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ImageBook>()
            .HasOne<Book>(b => b.Book)
            .WithMany(c => c.ImageBooks)
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Rating>()
            .HasOne(r => r.OrderItem)
            .WithMany(o => o.Ratings)
            .HasForeignKey(r => r.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Rating>()
            .HasOne(c => c.Customer)
            .WithMany(c => c.Ratings)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Book>()
            .Property(b => b.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasPrecision(18, 2);
    }
    

}