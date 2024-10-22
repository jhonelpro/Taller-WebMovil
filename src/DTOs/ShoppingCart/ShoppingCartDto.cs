using System.ComponentModel.DataAnnotations;
using api.src.Models;

namespace api.src.DTOs.Product
{
    public class ShoppingCartDto
    {
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public required string Name { get; set; } = string.Empty;

        [Range(0,100000000, ErrorMessage = "The price must be a positive number and less than 1000000000")]
        public required double Price { get; set; }
        
        [Required]
        public string ImageUrl { get; set; } = null!;
        public ProductType ProductType { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }
    }
}