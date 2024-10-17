using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.DTOs
{
    public class UpdateProductRequestDto
    {
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public string Name { get; set; } = string.Empty;

        [Range(0,100000000, ErrorMessage = "The price must be a positive number and less than 1000000000")]
        public double Price { get; set; }

        [Range(0,100000, ErrorMessage = "Stocl must be a non-negative integer less than 100,000")]
        public int Stock { get; set; }
        public string Image { get; set; } = string.Empty;
        public int ProductTypeId { get; set; } 
        public ProductType ProductType { get; set; } = null!;
    }
}