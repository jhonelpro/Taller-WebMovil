using System.Security.Claims;
using api.src.DTOs.User;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.src.Controller.MobileClient
{
    /// <summary>
    /// Controlador del Cliente Movil que maneja endpoints destinados a los usuarios autenticados.
    /// </summary>
    /// <remarks>
    /// Solo los usuarios autenticados con el rol de User pueden acceder a los endpoints de este controlador.
    /// El controlador permite realizar las siguientes acciones:
    /// <list type="bullet">
    /// <item> Cerrar sesión. </item>
    /// <item> Eliminar la cuenta del usuario con la sesion activa. </item>
    /// <item> Obtener las boletas correspondientes a cada una de sus compras. </item>
    /// <item> Obtener los productos disponibles para comprar. </item>
    /// </list>
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]

    public class MobileClientController: ControllerBase
    {
        /// <summary>
        /// Atributo de tipo UserManager<AppUser> que se encarga de manejar las operaciones de los usuarios.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Atributo de tipo SignInManager<AppUser> que se encarga de manejar las operaciones de inicio de sesión.
        /// </summary>
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Atributo IPasswordHasher<AppUser> que permite la inyeccion de dependencias.
        /// </summary>
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        /// <summary>
        /// Atributo de tipo ITicket que permite la inyeccion de dependencias.
        /// </summary>
        private readonly ITicket _ticket;

        /// <summary>
        /// Atributo de tipo IProductRepository que permite la inyeccion de dependencias.
        /// </summary>
        private readonly IProductRepository _productRepository;
        
        /// <summary>
        /// Constructor de la clase MobileClientController que recibe un objeto de tipo UserManager<AppUser>, SignInManager<AppUser>, IPasswordHasher<AppUser>, ITicket y IProductRepository.
        /// </summary>
        /// <param name="userManager">Parámetro de tipo UserManager<AppUser> que sirve para inicializar el atributo _userManager.</param>
        /// <param name="signInManager">Parámetro de tipo SignInManager que sirve para inicializar el atributo _signInManager.</param>
        /// <param name="passwordHasher">Parametro de tipo IPasswordHasher que sirve para inicializar el _passwordHasher.</param>
        /// <param name="ticket">Parametro de tipo ITicket que sirve para inicializar el atributo _ticket.</param>
        /// <param name="productRepository">Parámetro de tipo IProductRepository que sirve para inicializar el atributo _productRepository</param>
        public MobileClientController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher, ITicket ticket, IProductRepository productRepository)
        {
            //Inicializa el atributo _userManager con el objeto de tipo UserManager<AppUser> recibido.
            _userManager = userManager;
            //Inicializa el atributo _signInManager con el objeto de tipo SignInManager<AppUser> recibido.
            _signInManager = signInManager;
            //Inicializa el atributo _passwordHasher con el objeto de tipo IPasswordHasher<AppUser> recibido.
            _passwordHasher = passwordHasher;
            //Inicializa el atributo _ticket con el objeto de tipo ITicket recibido.
            _ticket = ticket;
            //Inicializa el atributo _productRepository con el objeto de tipo IProductRepository recibido.
            _productRepository = productRepository;
        }

        /// <summary>
        /// Endpoint para cerrar sesión en el sistema.
        /// </summary>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un mensaje que indica si se cerro sesión correctamente.
        /// </returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return BadRequest(new { message = "No active session found." });
                }

                await _signInManager.SignOutAsync();
                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout.", error = ex.Message });
            }
        }
        
        /// <summary>
        /// Endpoint que permite eliminar la cuenta del usuario con la sesion iniciada.
        /// </summary>
        /// <param name="deleteAccountDto">Parámetro que permite al usuario ingresar la contrasena y confimar la accion.</param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un mensaje que indica el resultado de la operacion.
        /// <list type="bullet">
        /// <item>200 OK con un mensaje de confirmacion.</item>
        /// <item>400 Bad Request si la contrasena no es valida.</item>
        /// <item>400 Bad Request si el usuario no ha iniciado sesion.</item>
        /// <item>404 Not Found si el usuario no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </list>
        /// </returns>
        [HttpDelete("deleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto deleteAccountDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (User.Identity?.IsAuthenticated != true) return BadRequest(new { message = "You have to login." });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return NotFound("User not found.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return NotFound("User not found.");

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, deleteAccountDto.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed) return BadRequest("Incorrect password.");

                if(deleteAccountDto.ConfirmDeleteAccount)
                {
                    var result = await _userManager.DeleteAsync(user);
                    await _signInManager.SignOutAsync();

                    if (result.Succeeded)
                    {
                        return Ok(new { message = "Account deleted successfully." });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error deleting account.", errors = result.Errors });
                    }
                }
                return BadRequest(new { message = "Account deletion was not confirmed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the account.", error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint para obtener las boletas correspondientes a cada una de las compras realizadas por el usuario.
        /// </summary>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un mensaje que indica el resultado de la operacion.
        /// <list type="bullet">
        /// <item>200 OK con la lista de boletas asociados al usuario con la sesion activa.</item>
        /// <item>400 Bad Request si el modelo de estado no es valido.</item>
        /// <item>404 Not Found si el usuario no fue encontrado.</item>
        /// <item>404 Not Found si las boletas no fueron encontrados.</item>
        /// </list>
        /// </returns>
        [HttpGet("getTickets")]
        public async Task<IActionResult> GetTickets()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var tickets = await _ticket.GetTickets(user.Id);

            if (tickets == null)
            {
                return NotFound("Tickets not found");
            }   

            return Ok(tickets);
        }

        /// <summary>
        /// Endpoint para obtener los productos disponibles para comprar.
        /// </summary>
        /// <param name="query"></param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un mensaje que indica el resultado de la operacion.
        /// <list type="bullet">
        /// <item>200 OK con la lista de productos disponibles.</item>
        /// <item>400 Bad Request si el tipo de producto es incorrecto.</item>
        /// <item>404 Not Found si el producto no fue encontrado.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </list>
        /// </returns>
        [HttpGet("getProductsClient")]
        public async Task<IActionResult> GetProductsClient([FromQuery] QueryObjectProductClient query)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var products = await _productRepository.GetProductsClient(query);
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
    }
}