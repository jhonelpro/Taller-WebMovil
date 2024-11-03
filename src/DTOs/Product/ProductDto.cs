using System.ComponentModel.DataAnnotations;
using api.src.Models;

namespace api.src.DTOs
{
    public class ProductDto
    {
        public required string Name { get; set; } = string.Empty;

        public required double Price { get; set; }

        public required int Stock { get; set; }

        public string ImageUrl { get; set; } = null!;
        
        public ProductType ProductType { get; set; } = null!;
    }
}