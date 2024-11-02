using System;
using Microsoft.EntityFrameworkCore;
using FoodApp.Models;

namespace FoodApp.Data
{
    public class FoodAppContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Detail> Details { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;database=netfood;user=root;password=123123",
                new MySqlServerVersion(new Version(8, 0, 21)));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasMany(u => u.Orders)
                .WithOne()
                .HasForeignKey(o => o.User_Id);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Details)
                .WithOne()
                .HasForeignKey(d => d.Order_Id);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Details)
                .WithOne()
                .HasForeignKey(d => d.Product_Id);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne()
                .HasForeignKey(p => p.Category_Id);

            modelBuilder.Entity<Detail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.Product_Id);

        }
    }
}