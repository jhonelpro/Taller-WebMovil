using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs.Product;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.src.Controller.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCart _shoppingCart;
        private readonly IShoppingCartItem _shoppingCartItem;
        private readonly UserManager<AppUser> _userManager;

        public ShoppingCartController(IProductRepository productRepository, IShoppingCart shoppingCart, 
        IShoppingCartItem shoppingCartItem, UserManager<AppUser> userManager)
        {
            _productRepository = productRepository;
            _shoppingCart = shoppingCart;
            _shoppingCartItem = shoppingCartItem;
            _userManager = userManager;
        }
        
        [HttpPost("AddTocart/{productId}/{quantity}")]
        public async Task<IActionResult> AddProductToShoppingCart([FromRoute] int productId,[FromRoute] int quantity)
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());
            
            var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                product.Quantity++;
            }
            else
            {
                cartItems.Add(new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
            return Ok("Product added to cart");
        }

        [HttpDelete("RemoveFromCart/{productId}")]
        public async Task<IActionResult> RemoveProductFromShoopingCart([FromRoute] int productId)
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());

            var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                cartItems.Remove(product);
                await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
                return Ok("Product removed from cart");
            }
            else
            {
                return NotFound(new { Message = "Product not found in cart" });
            }
        }

        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());
            var products = new List<ShoppingCartDto>();
            
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetProductById(item.ProductId);
                
                if (product != null)
                {
                    products.Add(product.ToShoppingCartDto(item));
                }
                else
                {
                    return NotFound(new { Message = "Product not found" });
                }
            }
            
            return Ok(products);
        }

        [HttpPut("UpdateCart/{productId}/{quantity}")]
        public async Task<IActionResult> UpdateProductInCart([FromRoute] int productId, [FromRoute] int quantity, bool? isIncrement)
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());
            var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                if (isIncrement == true)
                {
                    product.Quantity += quantity;
                }
                else if (isIncrement == false)
                {
                    product.Quantity -= quantity;
                }
                else
                {
                    product.Quantity = quantity;
                }
                
                await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
                return Ok("Product updated in cart");
            }
            else
            {
                return NotFound(new { Message = "Product not found in cart" });
            }
        }

        private List<ShoppingCartItem> GetCartItemsFromCookies()
        {
            var cartItems = new List<ShoppingCartItem>();
            var cartCookie = Request.Cookies["ShoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
            {
                cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
            }
            return cartItems ?? new List<ShoppingCartItem>();
        }

        private void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5)
            };
            Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
        }
    }
}