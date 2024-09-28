using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.DTOs
{
    public class ProductDto
    {
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public required string Name { get; set; } = string.Empty;

        [Range(0,100000000, ErrorMessage = "The price must be a positive number and less than 1000000000")]
        public required double Price { get; set; }

        [Range(0,100000, ErrorMessage = "Stocl must be a non-negative integer less than 100,000")]
        public required int Stock { get; set; }
        public string Image { get; set; } = string.Empty;
        public ProductType ProductType { get; set; } = null!;
    }
}