using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.src.Repositories
{
    public class ShoppingCartItemRespository : IShoppingCartItem
    {
        private readonly ApplicationDBContext _context;

        public ShoppingCartItemRespository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartItem> AddNewShoppingCartItem(int productId, int cartId, int quantity)
        {
            if (productId <= 0 || quantity <= 0 || cartId <= 0)
            {
                throw new Exception("Product id, quantity and cart id cannot be less than or equal to zero.");
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new Exception("Cart not found.");
            }

            var existingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                await _context.SaveChangesAsync();

                return existingCartItem;
            }
            
            var shoppingCartItem = new ShoppingCartItem
            {
                ProductId = productId,
                Product = product,
                CartId = cartId,
                Quantity = quantity
                
            };

            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await _context.SaveChangesAsync();

            return shoppingCartItem;
        }

        public async Task<ShoppingCartItem> AddShoppingCarItem(List<ShoppingCartItem> cartItems, int cartId)
        {
            if (cartId <= 0)
            {
                throw new ArgumentException("Cart ID must be greater than zero.", nameof(cartId));
            }

            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cart items cannot be null or empty.", nameof(cartItems));
            }

            // Obtiene el carrito y sus elementos en una sola consulta
            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.shoppingCartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new KeyNotFoundException("Cart not found.");
            }

            // Convierte los elementos existentes en un diccionario para búsqueda rápida
            var existingCartItems = shoppingCart.shoppingCartItems.ToDictionary(ci => ci.ProductId);

            foreach (var item in cartItems)
            {
                if (existingCartItems.TryGetValue(item.ProductId, out var existingCartItem))
                {        
                    existingCartItem.Quantity += item.Quantity;
                }
                else
                {
                    var product = await _context.Products
                        .Include(p => p.ProductType)
                        .FirstOrDefaultAsync(x => x.Id == item.ProductId);

                    if (product == null)
                    {
                        throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");
                    }

                    var newCartItem = new ShoppingCartItem
                    {
                        CartId = cartId,
                        ProductId = item.ProductId,
                        Product = product,
                        Quantity = item.Quantity
                    };
                    shoppingCart.shoppingCartItems.Add(newCartItem);
                }
            }

            await _context.SaveChangesAsync();

            return shoppingCart.shoppingCartItems.FirstOrDefault() ?? new ShoppingCartItem();
        }

        public async Task<bool> ClearShoppingCart(int cartId)
        {
            if (cartId <= 0)
            {
                throw new ArgumentException("Cart id cannot be less than or equal to zero.", nameof(cartId));
            }

            var shoppingCartItems = await GetShoppingCartItems(cartId);
            
            if (shoppingCartItems == null || !shoppingCartItems.Any())
            {
                throw new InvalidOperationException("Cart items not found.");
            }

            _context.ShoppingCartItems.RemoveRange(shoppingCartItems);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ShoppingCartItem> CreateShoppingCartItem(int productId, int cartId, int quantity)
        {
            if (productId <= 0 || cartId <= 0 || quantity <= 0)
            {
                throw new Exception("Product id, cart id and quantity cannot be less than or equal to zero.");
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var shoppingCartItem = new ShoppingCartItem
            {
                ProductId = productId,
                Product = product,
                CartId = cartId,
                Quantity = quantity
            };

            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await _context.SaveChangesAsync();

            return shoppingCartItem;
        }

        public async Task<ShoppingCartItem> GetShoppingCartItem(int productId)
        {
            if (productId <= 0)
            {
                throw new Exception("Product id cannot be less than or equal to zero.");
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            return shoppingCartItem;
        }

        public async Task<List<ShoppingCartItem>> GetShoppingCartItems(int cartId)
        {
            if (cartId <= 0)
            {
                throw new Exception("Cart id cannot be less than or equal to zero.");
            }

            var shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.CartId == cartId)
                .Include(s => s.shoppingCart)
                .Include(s => s.Product)
                .ToListAsync();

            if (shoppingCartItems == null)
            {
                throw new Exception("Cart not found.");
            }

            return shoppingCartItems;
        }

        public async Task<ShoppingCartItem> RemoveShoppingCartItem(int productId)
        {
            if (productId <= 0)
            {
                throw new Exception("Product id cannot be less than or equal to zero.");
            } 

            var existingCartItem = await GetShoppingCartItem(productId);

            if (existingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            _context.ShoppingCartItems.Remove(existingCartItem);
            await _context.SaveChangesAsync();

            return existingCartItem;
        }

        public async Task<ShoppingCartItem> UpdateShoppingCartItem(int productId, int quantity, bool? isIncrement)
        {
            if (productId <= 0 || quantity <= 0)
            {
                throw new Exception("Product id and quantity cannot be less than or equal to zero.");
            }

            var shoppingCartItem = GetShoppingCartItem(productId).Result;

            if (shoppingCartItem == null)
            {
                throw new Exception("Product not found.");
            }

            if (isIncrement == true)
            {
                shoppingCartItem.Quantity += quantity;
            }
            else if (!isIncrement == false)
            {
                shoppingCartItem.Quantity -= quantity;
            }
            else
            {
                shoppingCartItem.Quantity = quantity;
            }

            _context.ShoppingCartItems.Update(shoppingCartItem);
            await _context.SaveChangesAsync();

            return shoppingCartItem;
        }
    }
}