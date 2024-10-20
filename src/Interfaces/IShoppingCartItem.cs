using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IShoppingCartItem
    {
        Task<ShoppingCartItem> CreateShoppingCartItem(int productId, int cartId, int quantity);
        Task<ShoppingCartItem> AddShoppingCarItem(List<ShoppingCartItem> cartItems, int cartId);
        Task<ShoppingCartItem> AddNewShoppingCartItem(int productId, int cartId, int quantity);
        Task<ShoppingCartItem> UpdateShoppingCartItem(int productId, int quantity, bool? isIncrement);
        Task<ShoppingCartItem> RemoveShoppingCartItem(int productId);
        Task<List<ShoppingCartItem>> GetShoppingCartItems(int cartId);
        Task<ShoppingCartItem> GetShoppingCartItem(int productId);
    }
}