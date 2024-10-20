using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.src.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {

        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductType> ProductTypes { get; set; } = null!;
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public DbSet<Product_Purchase> Product_Purchases { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llamada al método base para aplicar configuraciones de Identity
            base.OnModelCreating(modelBuilder);

            // Seed roles into database
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
            
            modelBuilder.Entity<ShoppingCartItem>()
                .HasKey(pc => new { pc.CartId, pc.ProductId });

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(pc => pc.shoppingCart) // Relación con Cart
                .WithMany(c => c.shoppingCartItems)
                .HasForeignKey(pc => pc.CartId); // Clave foránea

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(pc => pc.Product) // Relación con Product
                .WithMany(p => p.shoppingCartItems)
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