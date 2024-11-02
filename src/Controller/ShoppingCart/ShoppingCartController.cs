using api.src.DTOs.Product;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
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

        /// <summary>
        /// Constructor de la clase ShoppingCartController.
        /// </summary>
        /// <param name="productRepository"></param>
        public ShoppingCartController(IProductRepository productRepository)
        {
            // Inicializa del atributo _productRepository.
            _productRepository = productRepository;

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

            // Guarda los productos en la cookie.
            await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
            return Ok("Product added to cart.");
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

            var cartItems = await Task.Run(() => GetCartItemsFromCookies());

            var product = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (product != null)
            {
                // Remueve el producto del carrito.
                cartItems.Remove(product);
                await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
                return Ok("Product removed from cart.");
            }
            else
            {
                return NotFound(new { Message = "Product not found in cart." });
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
                    return NotFound(new { Message = "Product not found." });
                }
            }
            
            return Ok(products.toCartDto());
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

                // Guarda los productos en la cookie.
                await Task.Run(() => SaveCartItemsToCookies(cartItems.ToList()));
                return Ok("Product updated in cart.");
            }
            else
            {
                return NotFound(new { Message = "Product not found in cart." });
            }
        }

        /// <summary>
        /// Método para obtener los productos en el carrito de compras desde las cookies.
        /// </summary>
        /// <returns>
        /// Retorna una lista de productos en el carrito de compras.
        /// </returns>
        private List<ShoppingCartItem> GetCartItemsFromCookies()
        {
            var cartItems = new List<ShoppingCartItem>();
            // Obtiene los productos del carrito de compras desde las cookies.
            var cartCookie = Request.Cookies["ShoppingCart"];
            if (!string.IsNullOrEmpty(cartCookie))
            {
                // Deserializa los productos del carrito de compras.
                cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
            }
            // Retorna los productos del carrito de compras.
            return cartItems ?? new List<ShoppingCartItem>();
        }

        /// <summary>
        /// Método para guardar los productos en el carrito de compras en las cookies.
        /// </summary>
        /// <param name="cartItems">Parámetro que representa el carrito de compras.</param>
        private void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems)
        {
            // Configura las opciones de la cookie.
            var options = new CookieOptions
            {
                // La cookie expira en 7 días. 
                Expires = DateTime.Now.AddDays(7),
            };
            // Guarda los productos en la cookie.
            Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
        }
    }
}