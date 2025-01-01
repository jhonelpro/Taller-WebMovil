using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.Purchase
{
    /// <summary>
    /// Controlador del proceso de venta y visualización de venta.
    /// </summary>
    /// <remarks>
    /// Solo los usuarios con el rol de User pueden acceder a los endpoints de este controlador.
    /// Este controlador permite realizar la compra de productos y obtener la boleta de la compra realizada.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class PurchaseController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo IShoppingCart que permite la inyección de dependencias.
        /// </summary>
        private readonly IShoppingCart _shoppingCart;

        /// <summary>
        /// Atributo de tipo IShoppingCartItem que permite la inyección de dependencias.
        /// </summary>
        private readonly IShoppingCartItem _shoppingCartItem;

        /// <summary>
        /// Atributo de tipo IPurchase que permite la inyección de dependencias.
        /// </summary>
        private readonly IPurchase _purchase;

        /// <summary>
        /// Atributo de tipo ISaleItem que permite la inyección de dependencias.
        /// </summary>
        private readonly ISaleItem _saleItem;

        /// <summary>
        /// Atributo de tipo UserManager<AppUser> que permite el manejo de usuarios a través de IdentityFramework.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Atributo de tipo ITicket que permite la inyección de dependencias.
        /// </summary>
        private readonly ITicket _ticket;

        /// <summary>
        /// Constructor de la clase PurchaseController que recibe un objeto de tipo IShoppingCart, IShoppingCartItem, IPurchase, ISaleItem, UserManager<AppUser> y ITicket.
        /// </summary>
        /// <param name="shoppingCart">Parámetro de tipo IShoppingCart que sirve para inicializar el atributo _shoppingCart</param>
        /// <param name="shoppingCartItem">Parámetro de tipo IShoppingCartItem que sirve para inicializar el atributo _shoppingCartItem</param>
        /// <param name="purchase">Parámetro de tipo IPurchase que sirve para inicializar el atributo _purchase</param>
        /// <param name="saleItem">Parámetro de tipo ISaleItem que sirve para inicializar el atributo _saleItem</param>
        /// <param name="userManager">Parámetro de tipo UserManager<AppUser> que sirve para inicializar el atributo _userManager</param>
        /// <param name="ticket">Parámetro de tipo ITicket que sirve para inicializar el atributo _ticket</param>
        public PurchaseController(IShoppingCart shoppingCart, IShoppingCartItem shoppingCartItem, IPurchase purchase, ISaleItem saleItem, UserManager<AppUser> userManager, ITicket ticket)
        {
            // Inicializa del atributo _shoppingCart.
            _shoppingCart = shoppingCart;
            // Inicializa del atributo _shoppingCartItem.
            _shoppingCartItem = shoppingCartItem;
            // Inicializa del atributo _purchase.
            _purchase = purchase;
            // Inicializa del atributo _userManager.
            _userManager = userManager;
            // Inicializa del atributo _saleItem.
            _saleItem = saleItem;
            // Inicializa del atributo _ticket.
            _ticket = ticket;
        }

        /// <summary>
        /// Endpoint para crear una nueva compra.
        /// </summary>
        /// <param name="purchaseDto">Parámetro de tipo CreatePurchaseDto que representa una venta</param>
        /// <returns>
        /// Retorna un objeto JSON con la información de la compra realizada.
        /// </returns>
        [HttpPost("NewPurchase")]
        public async Task<IActionResult> NewPurchase([FromBody] CreatePurchaseDto purchaseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "Please log in to complete the purchase." });
            }

            return await CreatePurchase(purchaseDto);
        }

        /// <summary>
        /// Método para la manejar la creación de un venta en el sistema.
        /// </summary>
        /// <param name="purchaseDto">Parámetro de tipo CreatePurchaseDto que representa una venta</param>
        /// <returns>
        /// Retorna un objeto JSON con la información de la compra realizada.
        /// <list type="Bullet">
        /// <item>200 Ok si la compra se realizó correctamente.</item>
        /// <item>400 Bad Request si el modelo de la compra no es válido.</item>
        /// <item>404 Not Found si no se encuentra el usuario.</item>
        /// <item>500 Internal Server Error si ocurre un error al intentar crear la compra.</item>
        /// </list>
        /// </returns>
        private async Task<IActionResult> CreatePurchase(CreatePurchaseDto purchaseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) 
                {
                    return BadRequest( new { message = "User not found."});
                }

                var purchase = purchaseDto.ToPurchaseFromCreateDto();

                if (purchase == null)
                {
                    return BadRequest( new { message = "Error creating purchase."});
                }
                
                var newPurchase = await _purchase.createPurchase(purchase, user); 

                if (newPurchase == null)
                {
                    return BadRequest( new { message = "Purchase not created."});
                }

                var cart = await _shoppingCart.GetShoppingCart(user.Id);

                if (cart == null)
                {
                    return BadRequest( new { message = "Cart not found."});
                }

                var shoppingCartItems = await _shoppingCartItem.GetShoppingCartItems(cart.Id);

                if (shoppingCartItems == null)
                {
                    return BadRequest( new { message = "Cart items not found."});
                }

                var saleItem = await _saleItem.createSaleItem(shoppingCartItems, newPurchase);

                if (saleItem == null)
                {
                    return BadRequest( new { message = "Sale item not created."});
                }

                var newTicket = await _ticket.CreateTicket(user, saleItem);

                if (newTicket == null)
                {
                    return BadRequest( new { message = "Ticket not created."});
                }

                var clearCart = await _shoppingCartItem.ClearShoppingCart(cart.Id);

                if (!clearCart)
                {
                    return BadRequest( new { message = "Cart not cleared."});
                }

                return Ok( newPurchase.Id );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }   
        }

        /// <summary>
        /// Endpoint para obtener la boleta de una compra.
        /// </summary>
        /// <param name="purchaseId">Parámetro que representa la id de la venta a la cual se le quiere crear una boleta.</param>
        /// <returns>
        /// Retorna un archivo PDF con la boleta de la compra realizada.
        /// <list type="Bullet">
        /// <item>200 Ok con la boleta de la compra realizada.</item>
        /// <item>400 Bad Request si el modelo de la compra no es válido.</item> 
        /// </returns>
        [HttpGet("GetPurchaseRecipt/{purchaseId:int}")]
        public async Task<IActionResult> GetPurchaseRecipt(int purchaseId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest(new { message = "User not found."});
                }

                if (purchaseId <= 0)
                {
                    return BadRequest( new { message = "Invalid purchase id."});
                }

                var purchase = await _purchase.getPurchase(purchaseId, user.Id);

                if (purchase == null)
                {
                    return BadRequest( new { message = "Purchase not found."});
                }

                var purchaseRecipt = await _purchase.getPurchaseRecipt(purchaseId, user.Id);

                // Se crea el nombre del archivo PDF.
                var fileName = $"BoletaCompra_{purchaseId}.pdf";

                // Se retorna el archivo PDF.
                return File(purchaseRecipt, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para obtener todas las compras realizadas por un usuario.
        /// </summary>
        /// <returns>
        /// Retorna un objeto JSON con la información de las compras realizadas por el usuario.
        /// <list type="Bullet">
        /// <item>200 Ok con la información de las compras realizadas.</item>
        /// <item>404 Not Found si no se encuentran compras realizadas.</item>
        /// <item>500 Internal Server Error si ocurre un error al intentar obtener la información de las compras realizadas.</item>
        /// </list>
        /// </returns>
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