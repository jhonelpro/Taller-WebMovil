using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    public class ShoppingCartRepository : IShoppingCart
    {
        private readonly ApplicationDBContext _context;

        public ShoppingCartRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> CreateShoppingCart(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var shoppingCart = new ShoppingCart
            {
                UserId = userId,
                Create_Date = DateTime.Now
            };

            await _context.ShoppingCarts.AddAsync(shoppingCart);
            await _context.SaveChangesAsync();

            return shoppingCart;
        }

        public async Task<ShoppingCart?> GetShoppingCart(string userId)
        {
            if (userId == null) 
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);

            return shoppingCart;
        }
    }
}