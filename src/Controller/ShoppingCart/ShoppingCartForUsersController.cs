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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class ShoppingCartForUsersController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly IShoppingCart _shoppingCart;
        private readonly IShoppingCartItem _shoppingCartItem;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICookieService _cookieService;

        public ShoppingCartForUsersController(IProductRepository productRepository, IShoppingCart shoppingCart, 
        IShoppingCartItem shoppingCartItem, UserManager<AppUser> userManager, ICookieService cookieService)
        {
            _productRepository = productRepository;
            _shoppingCart = shoppingCart;
            _shoppingCartItem = shoppingCartItem;
            _userManager = userManager;
            _cookieService = cookieService;
        }
        
        [HttpPost("CreateCartForUser")]
        public async Task<IActionResult> CreateCartForUser()
        { 
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var cartItems = _cookieService.GetCartItemsFromCookies();

            if (Request.Cookies["ShoppingCart"] != null)
            {
                return await HandleCartFromCookie(user, cartItems);
            }
            else
            {
                return await HandleNewCartCreation(user);
            }
        }

        [HttpPost("AddToCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> AddProductToShoppingCart([FromRoute] int productId, [FromRoute] int quantity)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try 
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest("Cart not found.");
                }

                var shoppingCartItem = await _shoppingCartItem.AddNewShoppingCartItem(productId, shoppingCart.Id, quantity);

                if (shoppingCartItem == null)
                {
                    return BadRequest("Failed to add product to cart.");
                }

                return Ok("Product added to cart.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
            
        }

        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try 
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest("Cart not found");
                }

                var cartItems = await _shoppingCartItem.GetShoppingCartItems(shoppingCart.Id);

                if (cartItems == null)
                {
                    return BadRequest("Cart is empty");
                }

                var products = new List<ShoppingCartDto>();

                foreach (var item in cartItems)
                {
                    var product = await _productRepository.GetProductById(item.ProductId);

                    if (product == null)
                    {
                        return BadRequest("Product not found");
                    }

                    products.Add(product.ToShoppingCartDto(item));
                }

                return Ok(products.toCartDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
            
        }

        [HttpPut("UpdateCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> UpdateProductInCart([FromRoute] int productId, [FromRoute] int quantity, bool? isIncrement)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest("Cart not found.");
                }

                var shoppingCartItem = await _shoppingCartItem.UpdateShoppingCartItem(productId, quantity, isIncrement);

                if (shoppingCartItem == null)
                {
                    return BadRequest("Failed to update product in cart.");
                }

                return Ok("Product updated in cart.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("RemoveFromCart/{productId:int}")]
        public async Task<IActionResult> RemoveProductFromShoopinCart([FromRoute] int productId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try 
            {
                var result = await _shoppingCartItem.RemoveShoppingCartItem(productId);

                if (result == null)
                {
                    return BadRequest("Failed to remove product from cart.");
                }

                return Ok("Product removed from cart");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }      
        }

        private async Task<IActionResult> HandleNewCartCreation(AppUser user)
        {
            try
            {
                var existingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (existingCart != null)
                {
                    return Ok("Cart already exists.");
                }

                var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest("Failed to create cart.");
                }

                return Ok("Cart created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

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
                        return BadRequest("Failed to add items to cart.");
                    }

                    _cookieService.ClearCartItemsInCookie();

                    return Ok("Items added to cart successfully.");
                }
                else
                {
                    return await CreateCartWhithItems(user, cartItems);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        private async Task<IActionResult> CreateCartWhithItems(AppUser user, List<ShoppingCartItem> cartItems)
        {
            try
            {
                if (cartItems == null || cartItems.Count == 0)
                {
                    return BadRequest(new { Message = "Cart is empty." });
                }

                var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

                if (shoppingCart == null)
                {
                    return BadRequest(new { Message = "Failed to create cart." });
                }

                var shoppingCartItems = await _shoppingCartItem.AddShoppingCarItem(cartItems, shoppingCart.Id);

                if (shoppingCartItems == null)
                {
                    return BadRequest(new { Message = "Failed to add items to cart." });
                }

                return Ok("Cart created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}