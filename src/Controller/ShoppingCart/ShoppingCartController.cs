using api.src.DTOs.Product;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
using api.src.Repositories;
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
        private readonly ICookieService _cookieService;

        public ShoppingCartController(IProductRepository productRepository, ICookieService cookieService)
        {
            _productRepository = productRepository;
            _cookieService = cookieService;
        }
        
        [HttpPost("AddTocart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> AddProductToShoppingCart([FromRoute] int productId,[FromRoute] int quantity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (quantity <= 0)
            {
                return BadRequest(new { Message = "Quantity must be greater than 0." });
            }

            try
            {
                var existingProduct = await _productRepository.GetProductById(productId);

                if (existingProduct == null)
                {
                    return NotFound(new { Message = "Product not found." });
                }

                var cartItems = await Task.Run(() => _cookieService.GetCartItemsFromCookies());
                
                var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

                if (product != null)
                {
                    product.Quantity += quantity;
                }
                else
                {
                    cartItems.Add(new ShoppingCartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Product = existingProduct
                    });
                }

                await Task.Run(() => _cookieService.SaveCartItemsToCookies(cartItems));
                return Ok("Product added to cart.");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpDelete("RemoveFromCart/{productId:int}")]
        public async Task<IActionResult> RemoveProductFromShoopingCart([FromRoute] int productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (productId <= 0)
            {
                return BadRequest(new { Message = "Product Id must be greater than 0." });
            }

            try
            {
                var existingProduct = await _productRepository.GetProductById(productId);

                if (existingProduct == null)
                {
                    return NotFound(new { Message = "Product not found." });
                }

                var cartItems = await Task.Run(() => _cookieService.GetCartItemsFromCookies());

                var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

                if (product != null)
                {
                    cartItems.Remove(product);
                    await Task.Run(() => _cookieService.SaveCartItemsToCookies(cartItems.ToList()));
                    return Ok("Product removed from cart.");
                }
                else
                {
                    return NotFound(new { Message = "Product not found in cart." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var cartItems = await Task.Run(() => _cookieService.GetCartItemsFromCookies());
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
                        return NotFound(new { Message = "Product not found." });
                    }
                }
                
                return Ok(products.toCartDto());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPut("UpdateCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> UpdateProductInCart([FromRoute] int productId, [FromRoute] int quantity, bool? isIncrement)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (quantity <= 0)
            {
                return BadRequest(new { Message = "Quantity must be greater than 0." });
            }

            try
            {
                var existingProduct = await _productRepository.GetProductById(productId);

                if (existingProduct == null)
                {
                    return NotFound(new { Message = "Product not found." });
                }

                var cartItems = await Task.Run(() => _cookieService.GetCartItemsFromCookies());
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
                    
                    await Task.Run(() => _cookieService.SaveCartItemsToCookies(cartItems.ToList()));
                    return Ok("Product updated in cart.");
                }
                else
                {
                    return NotFound(new { Message = "Product not found in cart." });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await Task.Run(() => _cookieService.ClearCartItemsInCookie());
                return Ok("Cart cleared.");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }
    }
}