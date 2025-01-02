using api.src.DTOs.Product;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
using api.src.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace api.src.Controller.Product
{
    /// <summary>
    /// Controlador para el carrito de compras de los usuarios autenticados dentro del sistema.
    /// </summary>
    /// <remarks>
    /// Solo los usuarios autenticados del tipo user pueden acceder a las funcionalidades de este controlador.
    /// Este controlador permite a los usuarios autenticados agregar, eliminar y actualizar productos en su carrito de compras
    /// interactuando con la base de datos.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class ShoppingCartForUsersController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo IProductRepository que permite la inyección de dependencias.
        /// </summary>
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Attributo de tipo IShoppingCart que permite la inyección de dependencias.
        /// </summary>
        private readonly IShoppingCart _shoppingCart;

        /// <summary>
        /// Attributo de tipo IShoppingCartItem que permite la inyección de dependencias.
        /// </summary>
        private readonly IShoppingCartItem _shoppingCartItem;

        /// <summary>
        /// Atributo de tipo UserManager<AppUser> que permite el manejo de usuarios a través de IdentityFramework.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;
        private readonly ICookieService _cookieService;

        /// <summary>
        /// Constructor de la clase ShoppingCartForUsersController que recibe un objeto de tipo IProductRepository, IShoppingCart, IShoppingCartItem, UserManager<AppUser>.
        /// </summary>
        /// <param name="productRepository">Parámetro de tipo IProductRepository que sirve para inicializar el atributo _productRepository</param>
        /// <param name="shoppingCart">Parámetro de tipo IShoppingCart que sirve para inicializar el atributo _shoppingCart</param>
        /// <param name="shoppingCartItem">Parámetro de tipo IShoppingCartItem que sirve para inicializar el atributo _shoppingCartItem</param>
        /// <param name="userManager">Parámetro de tipo UserManager<AppUser> que sirve para inicializar el atributo _userManager</param>
        public ShoppingCartForUsersController(IProductRepository productRepository, IShoppingCart shoppingCart, 
        IShoppingCartItem shoppingCartItem, UserManager<AppUser> userManager, ICookieService cookieService)
        {
            // Inicializa el atributo _productRepository.
            _productRepository = productRepository;
            // Inicializa el atributo _shoppingCart.
            _shoppingCart = shoppingCart;
            // Inicializa el atributo _shoppingCartItem.
            _shoppingCartItem = shoppingCartItem;
            // Inicializa el atributo _userManager.
            _userManager = userManager;
            _cookieService = cookieService;
        }
        
        /// <summary>
        /// Endpoint para crear un carrito de compras para un usuario autenticado.
        /// </summary>
        /// <returns>
        /// Retorna un mensaje de éxito si el carrito fue creado exitosamente.
        /// </returns>
        [HttpPost("CreateCartForUser")]
        public async Task<IActionResult> CreateCartForUser()
        { 
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest( new { message = "Usuario no encontrado" });
            }

            var cartItems = _cookieService.GetCartItemsFromCookies();

            // Si la cookie no está vacía, se crea el carrito con los productos de la cookie.
            if (Request.Cookies["ShoppingCart"] != null)
            {
                return await HandleCartFromCookie(user, cartItems);
            }
            // Si la cookie está vacía, se crea un carrito vacío.
            else
            {
                return await HandleNewCartCreation(user);
            }
        }

        /// <summary>
        /// Endpoint para agregar un producto al carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere agregar al carrito de compras.</param>
        /// <param name="quantity">Parámetro que representa la cantidad del producto.</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue agregado al carrito.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Product added to cart.</item>
        /// <item>400 Bad Request si el modelo no es válido.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito no fue encontrado.</item>
        /// <item>400 Bad Request si el producto no fue agregado al carrito.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list>
        /// </returns>
        [HttpPost("AddToCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> AddProductToShoppingCart([FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (quantity <= 0)
            {
                return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
            }

            try 
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuario no encontrado" });
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { message = "Carrito no encontrado" });
                }

                var shoppingCartItem = await _shoppingCartItem.AddNewShoppingCartItem(productId, shoppingCart.Id, quantity);

                if (shoppingCartItem == null)
                {
                    return BadRequest(new { message = "Producto no agregado al carrito" });
                }

                return Ok(new { message = "Porducto agregado al carrito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
            
        }

        /// <summary>
        /// Endpoint para obtener los productos del carrito de compras.
        /// </summary>
        /// <returns>
        /// Retorna un mensaje de éxito si los productos fueron obtenidos del carrito.
        /// <list type="Bullet">
        /// <item>200 OK con los productos del carrito.</item>
        /// <item>400 Bad Request si el modelo no es válido.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito está vacío.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </returns>
        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try 
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuario no encontrado" });
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { message = "Carrito no encontrado" });
                }

                var cartItems = await _shoppingCartItem.GetShoppingCartItems(shoppingCart.Id);

                if (cartItems == null)
                {
                    return BadRequest(new { message = "Carrito vacío" });
                }

                var products = new List<ShoppingCartDto>();

                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetProductById(item.ProductId);

                    if (product == null)
                    {
                        return BadRequest(new { message = "Producto no encontrado" });
                    }

                    products.Add(product.ToShoppingCartDto(item));
                }

                return Ok(products.toCartDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        /// <summary>
        /// Endpoint para actualizar la cantidad de un producto en el carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere agregar al carrito de compras.</param>
        /// <param name="quantity">Parámetro que representa la cantidad del producto que se quiere actualizar.</param>
        /// <param name="isIncrement">Parámetro que indica si se esta aumentando o disminuyendo la cantidad del producto.</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue actualizado en el carrito.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Product updated in cart.</item>
        /// <item>400 Bad Request si el modelo no es válido.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el producto no fue encontrado.</item>
        /// <item>400 Bad Request si el producto no fue actualizado en el carrito.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list>
        /// </returns>
        [HttpPut("UpdateCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> UpdateProductInCart([FromRoute] int productId, [FromRoute] int quantity, bool? isIncrement)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (quantity <= 0)
            {
                return BadRequest(new { message = "La cantidad debe ser mayor que 0" });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest(new { message = "Usuario no encontrado" });
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { message = "Carrito no encontrado" });
                }

                var shoppingCartItem = await _shoppingCartItem.UpdateShoppingCartItem(productId, quantity, isIncrement);

                if (shoppingCartItem == null)
                {
                    return BadRequest(new { message = "Producto no actualizado en el carrito" });
                }

                return Ok(new { message = "Producto actualizado en el carrito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para eliminar un producto del carrito de compras.
        /// </summary>
        /// <param name="productId">Parámetro que representa la id del producto que se quiere eliminar al carrito de compras.</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el producto fue eliminado del carrito.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Product removed from cart.</item>
        /// <item>400 Bad Request si el modelo no es válido.</item>
        /// <item>400 Bad Request si el producto no fue eliminado del carrito.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list>
        /// </returns>
        [HttpDelete("RemoveFromCart/{productId:int}")]
        public async Task<IActionResult> RemoveProductFromShoopinCart([FromRoute] int productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (productId <= 0)
            {
                return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
            }
            
            try 
            {
                var result = await _shoppingCartItem.RemoveShoppingCartItem(productId);

                if (result == null)
                {
                    return BadRequest(new { message = "Producto no eliminado del carrito" });
                }

                return Ok(new { message = "Producto eliminado del carrito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }      
        }

        /// <summary>
        /// Método para manejar la creación de un carrito de compras.
        /// </summary>
        /// <param name="user">Parámetro que representa el usuario al cual se le quiere crear un carrito de compras.</param>
        /// <returns>
        /// Retorna un mensaje de éxito si el carrito fue creado exitosamente.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Cart created successfully.</item>
        /// <item>200 OK con el mensaje Cart already exists.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito no fue creado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list>
        /// </returns>
        private async Task<IActionResult> HandleNewCartCreation(AppUser user)
        {
            try
            {
                var existingCart = await _shoppingCart.GetShoppingCart(user.Id);

                // Si el carrito ya existe, se retorna un mensaje de éxito.
                if (existingCart != null)
                {
                    return Ok(new { message = "Carrito ya existe" });
                }

                // Si el carrito no existe, se crea un carrito vacío.
                var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { message = "Error al crear el carrito" });
                }
            
                return Ok(new { message = "Carrito creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Método para manejar la creación de un carrito de compras con productos directo desde una cookie.
        /// </summary>
        /// <param name="user">Parámetro que representa al usuario al cual se le quiere crear un carrito de compras.</param>
        /// <param name="cartItems">Parámetro que representa los productos en el carrito de compras de una cookie</param>
        /// <returns>
        /// Retorna un mensaje de éxito si los productos fueron agregados al carrito.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Items added to cart successfully.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito no fue encontrado.</item>
        /// <item>400 Bad Request si los productos no fueron agregados al carrito.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list>
        /// </returns>
        private async Task<IActionResult> HandleCartFromCookie(AppUser user, List<ShoppingCartItem> cartItems)
        {
            try
            {
                var existingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (existingCart != null)
                {
                    var cartItemsFromDb = await _shoppingCartItem.AddShoppingCarItem(cartItems, existingCart.Id);

                    if (cartItemsFromDb == null)
                    {
                        return BadRequest(new { message = "Error al agregar productos al carrito" });
                    }

                    _cookieService.ClearCartItemsInCookie();

                    return Ok(new { message = "Productos agregados al carrito exitosamente" });
                }
                else
                {
                    return await CreateCartWhithItems(user, cartItems);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Método para crear un carrito de compras con productos.
        /// </summary>
        /// <param name="user">Parámetro que representa al usuario al cual se le quiere crear un carrito de compras</param>
        /// <param name="cartItems">Parámetro que representa los productos en el carrito de compras de una cookie</param>
        /// <returns>
        /// Retorna un mensaje de éxito si los productos fueron agregados al carrito.
        /// <list type="Bullet">
        /// <item>200 OK con el mensaje Cart created successfully.</item>
        /// <item>400 Bad Request si el usuario no fue encontrado.</item>
        /// <item>400 Bad Request si el carrito no fue creado.</item>
        /// <item>400 Bad Request si los productos no fueron agregados al carrito.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error interno.</item>
        /// </list> 
        /// </returns>
        private async Task<IActionResult> CreateCartWhithItems(AppUser user, List<ShoppingCartItem> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                return BadRequest(new { message = "Carrito esta vacio" });
            }

            try
            {
                var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { message = "Error al crear carrito" });
                }

                var shoppingCartItems = await _shoppingCartItem.AddShoppingCarItem(cartItems, shoppingCart.Id);

                if (shoppingCartItems == null)
                {
                    return BadRequest(new { message = "Error al agregar productos en el carrito" });
                }

                return Ok(new { message = "Carrrito creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}