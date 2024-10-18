using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;

namespace api.src.Repositories
{
    public class CartRepository : IShoppingCartInterface
    {
        private readonly ApplicationDBContext _context;

        public CartRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<Cart> AddNewCart(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("UserId cannot be null or empty");
            
            var newCart = new Cart
            {
                UserId = userId,
                Create_Date = DateTime.Now
            };

            await _context.Carts.AddAsync(newCart);
            await _context.SaveChangesAsync();

            return newCart;

        }

        public async Task<Cart> GetCartById(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null) throw new Exception("Cart not found");

            return cart;
        }
    }
}