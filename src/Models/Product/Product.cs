using System.ComponentModel.DataAnnotations;

namespace api.src.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public required double Price { get; set; }

        public required int Stock { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int ProductTypeId { get; set; }  // Cambiado a ProductTypeId
        public ProductType ProductType { get; set; } = null!;

        public List<ShoppingCartItem> shoppingCartItems { get; } = [];
        public List<SaleItem> SaleItems { get; } = [];
    }
}