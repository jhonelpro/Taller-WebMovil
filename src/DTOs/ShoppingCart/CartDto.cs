using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs.Product;

namespace api.src.DTOs.ShoppingCart
{
    public class CartDto
    {
        public double Cart_TotalPrice { get; set; }

        public List<ShoppingCartDto> CartItems { get; set; } = new List<ShoppingCartDto>();
    }
}