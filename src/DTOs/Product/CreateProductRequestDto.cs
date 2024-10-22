using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs
{
    public class CreateProductRequestDto
    {
        [Required]
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0,100000000, ErrorMessage = "The price must be a positive number and less than 1000000000")]
        public double Price { get; set; }

        [Required]
        [Range(0,100000, ErrorMessage = "Stocl must be a non-negative integer less than 100,000")]
        public int Stock { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
        
        [Required]
        public int ProductTypeId { get; set; } 
    }
}