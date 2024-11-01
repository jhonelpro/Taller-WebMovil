using System.ComponentModel.DataAnnotations;
using api.src.Models;

namespace api.src.DTOs.Product
{
    public class ShoppingCartDto
    {
        public required string Name { get; set; } = string.Empty;

        public required double Price { get; set; }
        
        public string ImageUrl { get; set; } = null!;
        public ProductType ProductType { get; set; } = null!;

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }
    }
}