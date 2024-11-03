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
    /// <summary>
    /// Controlador para el carrito de compras.
    /// </summary>
    /// <remarks>
    /// Este controlador permite agregar, eliminar y actualizar productos en el carrito de compras.
    /// No interactúa directamente con la base de datos para la opciones del carrito; en su lugar, utiliza cookies para almacenar 
    /// la información del carrito de compras del usuario.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo IProductRepository que permite la inyección de dependencias.
        /// </summary>
        private readonly IProductRepository _productRepository;
        private readonly ICookieService _cookieService;

        public ShoppingCartController(IProductRepository productRepository, ICookieService cookieService)
        {
            // Inicializa del atributo _productRepository.
            _productRepository = productRepository;
            _cookieService = cookieService;
        }

        /// <summary>
        /// Endpoint para agregar un producto al carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere agregar al carrito.</param>
        /// <param name="quantity">Parámetro que representa la cantidad del producto que se quiere agregar.</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue agregado al carrito.
        /// </returns>
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

        /// <summary>
        /// Endpoint para remover un producto del carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere eliminar del carrito</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue eliminado del carrito.
        /// </returns>
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

        /// <summary>
        /// Endpoint para obtener los productos en el carrito de compras.
        /// </summary>
        /// <returns>
        /// Retorna una lista de productos en el carrito de compras.
        /// Retorna un mensaje de error si no se encuentran productos en el carrito.
        /// </returns>
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

        /// <summary>
        /// Endpoint para actualizar la cantidad de un producto en el carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere agregar al carrito.</param>
        /// <param name="quantity">Parámetro que representa la cantidad del producto que se quiere agregar.</param>
        /// <param name="isIncrement">Parámetro que indica si se debe aumentar o disminuir la cantidad del producto</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue actualizado en el carrito.
        /// Retorna un mensaje de error si el producto no fue encontrado en el carrito.
        /// </returns>
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