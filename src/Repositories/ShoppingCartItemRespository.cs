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

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new Exception("Cart not found.");
            }

            var existingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == productId);

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
                throw new Exception("Cart id cannot be less than or equal to zero.");
            }

            if (cartItems == null)
            {
                throw new Exception("Cart items cannot be null.");
            }

            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart == null)
            {
                throw new Exception("Cart not found.");
            }

            var existingCartItems = await _context.ShoppingCartItems.Where(x => x.CartId == cartId).ToListAsync();

            if (existingCartItems != null)
            {
                foreach (var item in cartItems)
                {
                    var existingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += item.Quantity;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            var shoppingCartItem = new ShoppingCartItem
            {
                CartId = cartId
            };

            foreach (var item in cartItems)
            { 
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                if (product == null)
                {
                    throw new Exception("Product not found.");
                }

                var existingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (existingCartItem == null)
                {
                    shoppingCartItem.Product = product;
                    shoppingCartItem.ProductId = item.ProductId;
                    shoppingCartItem.Quantity = item.Quantity;
            
                    await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
                    await _context.SaveChangesAsync();
                }else
                {
                    existingCartItem.Quantity += item.Quantity;
                    await _context.SaveChangesAsync();
                }
                
            }

            return shoppingCartItem;
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

            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

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

        public List<ShoppingCartItem> GetCartItemsFromCookies(HttpRequest request)
        {
            var cartItems = new List<ShoppingCartItem>();

            var cartCookie = request.Cookies["ShoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
            {
                cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
            }

            return cartItems ?? new List<ShoppingCartItem>();
        }

        public async Task<ShoppingCartItem> GetShoppingCartItem(int productId)
        {
            if (productId <= 0)
            {
                throw new Exception("Product id cannot be less than or equal to zero.");
            }

            var shoppingCartItem = await _context.ShoppingCartItems.FirstOrDefaultAsync(x => x.ProductId == productId);

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

            var shoppingCartItems = await _context.ShoppingCartItems.Where(x => x.CartId == cartId).ToListAsync();

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