using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.Purchase
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class PurchaseController : ControllerBase
    {
        private readonly IShoppingCart _shoppingCart;
        private readonly IShoppingCartItem _shoppingCartItem;
        private readonly IPurchase _purchase;
        private readonly ISaleItem _saleItem;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicket _ticket;

        public PurchaseController(IShoppingCart shoppingCart, IShoppingCartItem shoppingCartItem, IPurchase purchase, ISaleItem saleItem, UserManager<AppUser> userManager, ITicket ticket)
        {
            _shoppingCart = shoppingCart;
            _shoppingCartItem = shoppingCartItem;
            _purchase = purchase;
            _userManager = userManager;
            _saleItem = saleItem;
            _ticket = ticket;
        }

        [HttpPost("NewPurchase")]
        public async Task<IActionResult> NewPurchase([FromBody] CreatePurchaseDto purchaseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "Please log in to complete the purchase." });
            }

            return await CreatePurchase(purchaseDto);
        }

        private async Task<IActionResult> CreatePurchase(CreatePurchaseDto purchaseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) 
                {
                    return BadRequest("User not found.");
                }

                var purchase = purchaseDto.ToPurchaseFromCreateDto();

                if (purchase == null)
                {
                    return BadRequest("Error creating purchase.");
                }
                
                var newPurchase = await _purchase.createPurchase(purchase, user); 

                if (newPurchase == null)
                {
                    return BadRequest("Purchase not created.");
                }

                var cart = await _shoppingCart.GetShoppingCart(user.Id);

                if (cart == null)
                {
                    return BadRequest("Cart not found.");
                }

                var shoppingCartItems = await _shoppingCartItem.GetShoppingCartItems(cart.Id);

                if (shoppingCartItems == null)
                {
                    return BadRequest("Cart items not found.");
                }

                var saleItem = await _saleItem.createSaleItem(shoppingCartItems, newPurchase);

                if (saleItem == null)
                {
                    return BadRequest("Sale item not created.");
                }

                var newTicket = await _ticket.CreateTicket(user, saleItem);

                if (newTicket == null)
                {
                    return BadRequest("Ticket not created.");
                }

                var clearCart = await _shoppingCartItem.ClearShoppingCart(cart.Id);

                if (!clearCart)
                {
                    return BadRequest("Cart not cleared.");
                }

                return Ok("Purchase created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }   
        }

        [HttpGet("GetPurchaseRecipt/{purchaseId:int}")]
        public async Task<IActionResult> GetPurchaseRecipt(int purchaseId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                if (purchaseId <= 0)
                {
                    return BadRequest("Invalid purchase id.");
                }

                var purchase = await _purchase.getPurchase(purchaseId, user.Id);

                if (purchase == null)
                {
                    return BadRequest("Purchase not found.");
                }

                var purchaseRecipt = await _purchase.getPurchaseRecipt(purchaseId, user.Id);

                var fileName = $"BoletaCompra_{purchaseId}.pdf";
                return File(purchaseRecipt, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("GetPurchases")]
        public async Task<IActionResult> GetPurchases()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try{
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var purchases = await _saleItem.GetPurchasesAsync(user.Id);

                if (purchases == null)
                {
                    return BadRequest("Purchases not found.");
                }   

                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}