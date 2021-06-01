using System;
using System.Collections.Generic;
using pwned_shop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace pwned_shop.Data
{
    public class PwnedShopDb : DbContext
    {
        protected IConfiguration configuration;
        public PwnedShopDb(DbContextOptions<PwnedShopDb> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Review>().HasKey(r => new { r.UserId, r.ProductId });
            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Rating)
                        .WithMany(r => r.Products)
                        .HasForeignKey(p => p.ESRBRating);
            modelBuilder.Entity<Order>()
                        .HasOne(o => o.Discount)
                        .WithMany(d => d.Orders)
                        .HasForeignKey(o => o.PromoCode)
                        .IsRequired(false);
            modelBuilder.Entity<Cart>().HasKey(c => new { c.UserId, c.ProductId });
            modelBuilder.Entity<Cart>().HasIndex(c => c.UserId);
        }
    }
}
