using BBShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BBShop.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Store> Stores { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShoppingCart>().HasKey(sc => sc.CartId);
        modelBuilder.Entity<CartItem>().HasKey(ci => ci.CartItemId);
        modelBuilder.Entity<Store>().HasKey(s => s.StoreId);
        modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
        modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
        modelBuilder.Entity<OrderItem>().HasKey(oi => oi.OrderItemId);

        modelBuilder.Entity<Store>()
            .HasOne(s => s.User)
            .WithMany(u => u.Stores)
            .HasForeignKey(s => s.UserId);
    }
}