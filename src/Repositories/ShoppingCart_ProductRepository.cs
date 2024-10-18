using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    
    public class ShoppingCart_ProductRepository : IShoppingCart_ProductInterface
    {
        private readonly ApplicationDBContext _context;

        public ShoppingCart_ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Product_Cart> addProductToCart(int cartId, int productId, int quantity)
        {
            
            var productCart = new Product_Cart
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = quantity
            };

            await _context.Product_Carts.AddAsync(productCart);
            await _context.SaveChangesAsync();
            
            return productCart;
        }

        public async Task<Product_Cart> getProductCartById(int cartId, int productId)
        {
            var productCart = await _context.Product_Carts.FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);

            if (productCart == null) throw new Exception("Product not found in cart");

            return productCart;

        }
    }
}