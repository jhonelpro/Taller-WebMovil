using api.src.Models;
using api.src.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.src.Data
{
    /// <summary>
    /// Contexto de la base de datos para la aplicación, que extiende de IdentityDbContext y maneja las entidades relacionadas con usuarios, roles y modelos de negocio.
    /// </summary>
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        /// <summary>
        /// Constructor de la clase ApplicationDBContext que recibe las opciones de configuración de la base de datos.
        /// </summary>
        /// <param name="dbContextOptions">Parametro de tipo DbContextOptions para configurar el contexto de base de datos.</param>
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        /// <summary>
        /// Tabla de productos.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Tabla de tipos de productos.
        /// </summary>
        public DbSet<ProductType> ProductTypes { get; set; } = null!;

        /// <summary>
        /// Tabla de carritos de compras.
        /// </summary>
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;

        /// <summary>
        /// Tabla de compras.
        /// </summary>
        public DbSet<Purchase> Purchases { get; set; } = null!;

        /// <summary>
        /// Tabla de ítems de carrito de compras.
        /// </summary>
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;

        /// <summary>
        /// Tabla de ítems de venta.
        /// </summary>
        public DbSet<SaleItem> SaleItems { get; set; } = null!;

        /// <summary>
        /// Tabla de boletas.
        /// </summary>
        public DbSet<Ticket> Tickets { get; set; } = null!;

        /// <summary>
        /// Configura las relaciones y el mapeo de las entidades de negocio en la base de datos.
        /// </summary>
        /// <param name="modelBuilder">Parametro de tipo ModelBuilder usado para definir el mapeo de las entidades.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llama al método base para aplicar configuraciones predeterminadas de Identity
            base.OnModelCreating(modelBuilder);

            // Roles predeterminados que se agregarán en la base de datos
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" }
            };

            // Inserta los roles en la base de datos
            modelBuilder.Entity<IdentityRole>().HasData(roles);
            
            // Configuración de la entidad ShoppingCartItem
            modelBuilder.Entity<ShoppingCartItem>()
                .HasKey(pc => new { pc.CartId, pc.ProductId }); // Clave compuesta

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(pc => pc.shoppingCart) // Relación con el carrito
                .WithMany(c => c.shoppingCartItems)
                .HasForeignKey(pc => pc.CartId); // Clave foránea

            modelBuilder.Entity<ShoppingCartItem>()
                .HasOne(pc => pc.Product) // Relación con el producto
                .WithMany(p => p.shoppingCartItems)
                .HasForeignKey(pc => pc.ProductId); // Clave foránea

            // Configuración de la entidad SaleItem
            modelBuilder.Entity<SaleItem>()
                .HasKey(pp => new { pp.PurchaseId, pp.ProductId }); // Clave compuesta

            modelBuilder.Entity<SaleItem>()
                .HasOne(pp => pp.Purchase) // Relación con la compra
                .WithMany(p => p.SaleItems)
                .HasForeignKey(pp => pp.PurchaseId); // Clave foránea

            modelBuilder.Entity<SaleItem>()
                .HasOne(pp => pp.Product) // Relación con el producto
                .WithMany(p => p.SaleItems)
                .HasForeignKey(pp => pp.ProductId); // Clave foránea
        }
    }
}