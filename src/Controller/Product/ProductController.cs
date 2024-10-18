using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
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
        private readonly IShoppingCartInterface _shoppingCartRepository;
        private readonly IShoppingCart_ProductInterface _shoppingCartProductRepository;

        public ProductController(IProductRepository productRepository, IShoppingCartInterface shoppingCartRepository, IShoppingCart_ProductInterface shoppingCartProductRepository)
        {
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _shoppingCartProductRepository = shoppingCartProductRepository;
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

        [HttpPost("AddTocart/{productId}")]
        public async Task<IActionResult> AddProductToShoppingCart([FromRoute] int productId)
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());
            
            var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                product.Quantity++;
            }
            else
            {
                cartItems.Add(new Product_Cart
                {
                    ProductId = productId,
                    Quantity = 1
                });
            }

            await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
            return Ok("Product added to cart");
        }

        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            var cartItems = await Task.Run(() => GetCartItemsFromCookies());
            var products = new List<ProductDto>();
            
            foreach (var item in cartItems)
            {
                var product = await _productRepository.GetProductById(item.ProductId);
                
                if (product != null)
                {
                    products.Add(product.ToProductDto());
                }
                else
                {
                    return NotFound(new { Message = "Product not found" });
                }
            }
            
            return Ok(products);
        }

        private List<Product_Cart> GetCartItemsFromCookies()
        {
            var cartItems = new List<Product_Cart>();
            var cartCookie = Request.Cookies["ShoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
            {
                cartItems = JsonConvert.DeserializeObject<List<Product_Cart>>(cartCookie);
            }
            return cartItems ?? new List<Product_Cart>();
        }

        private void SaveCartItemsToCookies(List<Product_Cart> cartItems)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7)
            };
            Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
        }
    }
}