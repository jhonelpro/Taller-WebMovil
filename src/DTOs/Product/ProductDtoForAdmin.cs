using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.DTOs.Product
{
    public class ProductDtoForAdmin
    {
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        public required double Price { get; set; }

        public required int Stock { get; set; }
        
        public string ImageUrl { get; set; } = null!;
        
        public ProductType ProductType { get; set; } = null!;
    }
}