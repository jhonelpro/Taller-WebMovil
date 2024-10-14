using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Models.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.src.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductType> ProductTypes { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Product_Cart> Product_Carts { get; set; } = null!;
        public DbSet<Product_Purchase> Product_Purchases { get; set; } = null!;

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product_Cart>()
                .HasKey(pc => new { pc.CartId, pc.ProductId }); // Clave compuesta

            modelBuilder.Entity<Product_Cart>()
                .HasOne(pc => pc.Cart) // Relación con Cart
                .WithMany(c => c.Product_Carts)
                .HasForeignKey(pc => pc.CartId); // Clave foránea

            modelBuilder.Entity<Product_Cart>()
                .HasOne(pc => pc.Product) // Relación con Product
                .WithMany(p => p.Product_Carts)
                .HasForeignKey(pc => pc.ProductId); // Clave foránea

            modelBuilder.Entity<Product_Purchase>()
            .HasKey(pp => new { pp.PurchaseId, pp.ProductId }); // Clave compuesta

            modelBuilder.Entity<Product_Purchase>()
                .HasOne(pp => pp.Purchase) // Relación con Purchase
                .WithMany(p => p.product_Purchases)
                .HasForeignKey(pp => pp.PurchaseId); // Clave foránea

            modelBuilder.Entity<Product_Purchase>()
                .HasOne(pp => pp.Product) // Relación con Product
                .WithMany(p => p.product_Purchases)
                .HasForeignKey(pp => pp.ProductId); // Clave foránea
        }
    }
}