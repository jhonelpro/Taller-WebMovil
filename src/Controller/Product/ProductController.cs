using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.DTOs.Product;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    //Agregar Authorize
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;


        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var products = await _productRepository.GetAvailableProducts(query);
                return Ok(products);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "Product not found")
                {
                    return NotFound(new { Message = ex.Message });
                }
                else if (ex.Message == "Product Type incorrect")
                {
                    return BadRequest(new { Message = ex.Message });
                }
                else
                {
                    return StatusCode(500, new { Message = "An error occurred while processing your request." });
                }
            }
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
        public async Task<IActionResult> RemoveProductFromShoopinCart([FromRoute] int productId)
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
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
        }
    }
}