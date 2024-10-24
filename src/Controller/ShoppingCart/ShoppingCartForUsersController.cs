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
    [Authorize(Roles = "User")]
    public class ShoppingCartForUsersController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly IShoppingCart _shoppingCart;
        private readonly IShoppingCartItem _shoppingCartItem;
        private readonly UserManager<AppUser> _userManager;

        public ShoppingCartForUsersController(IProductRepository productRepository, IShoppingCart shoppingCart, 
        IShoppingCartItem shoppingCartItem, UserManager<AppUser> userManager)
        {
            _productRepository = productRepository;
            _shoppingCart = shoppingCart;
            _shoppingCartItem = shoppingCartItem;
            _userManager = userManager;
        }
        
        [HttpPost("CreateCartForUser")]
        public async Task<IActionResult> CreateCartForUser()
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var cartItems = GetCartItemsFromCookies();

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (productId == 0 || quantity == 0)
            {
                return BadRequest("Product not found");
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

            if (shoppingCart == null)
            {
                shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);
            }

            var shoppingCartItem = await _shoppingCartItem.AddNewShoppingCartItem(productId, shoppingCart.Id, quantity);

            if (shoppingCartItem == null)
            {
                return BadRequest("Failed to add product to cart");
            }

            return Ok("Product added to cart");
        }

        [HttpGet("ProductsInCart")]
        public async Task<IActionResult> GetProductsInCart()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return Ok(products);
        }

        [HttpPut("UpdateCart/{productId:int}/{quantity:int}")]
        public async Task<IActionResult> UpdateProductInCart([FromRoute] int productId, [FromRoute] int quantity, bool? isIncrement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (productId == 0 || quantity == 0)
            {
                return BadRequest("Product not found");
            }

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

            var shoppingCartItem = await _shoppingCartItem.UpdateShoppingCartItem(productId, quantity, isIncrement);

            if (shoppingCartItem == null)
            {
                return BadRequest("Failed to update product in cart");
            }

            return Ok("Product updated in cart");
        }

        [HttpDelete("RemoveFromCart/{productId:int}")]
        public async Task<IActionResult> RemoveProductFromShoopinCart([FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (productId == 0)
            {
                return BadRequest("Product not found");
            }

            var result = await _shoppingCartItem.RemoveShoppingCartItem(productId);

            if (result == null)
            {
                return BadRequest("Failed to remove product from cart");
            }

            return Ok("Product removed from cart");
        }

        private async Task<IActionResult> HandleNewCartCreation(AppUser user)
        {
            var existingCart = await _shoppingCart.GetShoppingCart(user.Id);

            if (existingCart != null)
            {
                return Ok("Cart already exists");
            }

            var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

            if (shoppingCart == null)
            {
                return BadRequest("Failed to create cart");
            }

            return Ok("Cart created successfully"); 
        }

        private async Task<IActionResult> HandleCartFromCookie(AppUser user, List<ShoppingCartItem> cartItems)
        {
            var existingCart = await _shoppingCart.GetShoppingCart(user.Id);

            if (existingCart != null)
            {
                var cartItemsFromDb = await _shoppingCartItem.AddShoppingCarItem(cartItems, existingCart.Id);

                if (cartItemsFromDb == null)
                {
                    return BadRequest("Failed to add items to cart");
                }

                return Ok("Items added to cart successfully");
            }
            else
            {
                return await CreateCartWhithItems(user, cartItems);
            }
        }

        private async Task<IActionResult> CreateCartWhithItems(AppUser user, List<ShoppingCartItem> cartItems)
        {
            if (cartItems == null || cartItems.Count == 0)
            {
                return BadRequest(new { Message = "Cart is empty" });
            }

            var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

            if (shoppingCart == null)
            {
                return BadRequest(new { Message = "Failed to create cart" });
            }

            var shoppingCartItems = await _shoppingCartItem.AddShoppingCarItem(cartItems, shoppingCart.Id);

            if (shoppingCartItems == null)
            {
                return BadRequest(new { Message = "Failed to add items to cart" });
            }

            return Ok("Cart created successfully");
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
    }
}