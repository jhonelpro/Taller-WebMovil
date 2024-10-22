using System.ComponentModel.DataAnnotations;

namespace api.src.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0,100000000, ErrorMessage = "The price must be a positive number and less than 1000000000")]
        public required double Price { get; set; }

        [Required]
        [Range(0,100000, ErrorMessage = "Stocl must be a non-negative integer less than 100,000")]
        public required int Stock { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int ProductTypeId { get; set; }  // Cambiado a ProductTypeId
        public ProductType ProductType { get; set; } = null!;

        public List<ShoppingCartItem> shoppingCartItems { get; } = [];
        public List<SaleItem> SaleItems { get; } = [];
    }
}