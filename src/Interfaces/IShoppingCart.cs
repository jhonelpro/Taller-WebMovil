using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IShoppingCart
    {
        Task<ShoppingCart> CreateShoppingCart(string userId);
        Task<ShoppingCart?> GetShoppingCart(string userId);
    }
}