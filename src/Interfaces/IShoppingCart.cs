
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IShoppingCart
    {
        Task<ShoppingCart> CreateShoppingCart(string userId);
        Task<ShoppingCart?> GetShoppingCart(string userId);
    }
}