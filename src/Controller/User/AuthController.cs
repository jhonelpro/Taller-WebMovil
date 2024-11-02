using api.src.DTOs.Auth;
using api.src.DTOs.User;
using api.src.Interfaces;
using api.src.Models.User;
using api.src.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.src.Controller.Product;
using Microsoft.AspNetCore.Authorization;

namespace api.src.Controller
{
    /// <summary>
    /// Controlador de autenticación de usuarios.
    /// </summary>
    /// <remarks>
    /// Este controlador se encarga de manejar las peticiones de autenticación de usuarios.
    /// las acciones que se pueden realizar son: registro, login y logout.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo UserManager<AppUser> que se encarga de manejar las operaciones de los usuarios.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Atributo de tipo ITokenService que se encarga de manejar la creación de tokens.
        /// </summary>
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Atributo de tipo SignInManager<AppUser> que se encarga de manejar las operaciones de inicio de sesión.
        /// </summary>
        private readonly SignInManager<AppUser> _signInManager;

        /// <summary>
        /// Atributo de tipo IShoppingCart que se encarga de manejar las operaciones de carrito de compras.
        /// </summary>
        private readonly IShoppingCart _shoppingCart;

        /// <summary>
        /// Constructor de la clase AuthController que recibe un objeto de tipo UserManager<AppUser>, ITokenService, SignInManager<AppUser>, IShoppingCart.
        /// </summary>
        /// <param name="userManager">Parámetro de tipo UserManager<AppUser> que sirve para inicializar el atributo _userManager</param>
        /// <param name="tokenService">Parámetro de tipo ITokenService que sirve para inicializar el atributo _tokenService</param>
        /// <param name="signInManager">Parámetro de tipo SignInManager que sirve para inicializar el atributo _signInManager</param>
        /// <param name="shoppingCart">Parámetro de tipo IShoppingCart que sirve para inicializar el atributo _shoppingCart</param>
        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager, IShoppingCart shoppingCart)
        {
            // Inicializa el atributo _userManager con el valor del parámetro userManager
            _userManager = userManager;
            // Inicializa el atributo _tokenService con el valor del parámetro tokenService
            _tokenService = tokenService;
            // Inicializa el atributo _signInManager con el valor del parámetro signInManager
            _signInManager = signInManager;
            // Inicializa el atributo _shoppingCart con el valor del parámetro shoppingCart
            _shoppingCart = shoppingCart;
        }

        /// <summary>
        /// Endpoint para registrar un nuevo usuario.
        /// </summary>
        /// <param name="registerDto">Parámetro que representa un usuario el cual se registrara en el sistema.</param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un objeto de tipo NewUserDto con los datos del usuario y el token de autenticación.
        /// </returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (await _userManager.Users.AnyAsync(p => p.Email == registerDto.Email)) return BadRequest("Email already exists.");

                if (string.IsNullOrEmpty(registerDto.Rut)) return BadRequest("RUT is required.");

                if (await _userManager.Users.AnyAsync(p => p.Rut == registerDto.Rut)) return BadRequest("Rut already exists.");

                if (!RutValidations.IsValidRut(registerDto.Rut)) return BadRequest("Invalid Rut format or verification digit.");

                if (registerDto.DateOfBirth >= DateTime.Now) return BadRequest("Date of birth must be in the past.");

                if (string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.ConfirmPassword)) return BadRequest("Password is required.");

                if (!string.Equals(registerDto.Password, registerDto.ConfirmPassword, StringComparison.Ordinal)) return BadRequest("Passwords do not match.");

                var user = new AppUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    Rut = registerDto.Rut,
                    Name = registerDto.Name,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    IsActive = 1
                };
                
                var createUser = await _userManager.CreateAsync(user, registerDto.Password);

                // Si el usuario se crea correctamente
                if (createUser.Succeeded)
                {
                    // Asignar rol de usuario
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (roleResult.Succeeded)
                    {
                        // Crear carrito de compras para el usuario
                        await _shoppingCart.CreateShoppingCart(user.Id);

                        return Ok(new NewUserDto
                        {
                            UserName = user.UserName!,
                            Email = user.Email!,
                            Token = _tokenService.CreateToken(user)
                        });
                    }

                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }else
                {
                    return StatusCode(500, createUser.Errors);
                }

            } 
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Endpoint para iniciar sesión en el sistema.
        /// </summary>
        /// <param name="loginDto">Parámetro que representa un usuario que quiere iniciar sesión en el sistema.</param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un objeto de tipo NewUserDto con los datos del usuario y el token de autenticación.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try {

                if(!ModelState.IsValid) return BadRequest(ModelState);

                // Buscar usuario por nombre de usuario
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
                if(user == null) return Unauthorized("Invalid username or password.");

                // Verificar si la contraseña es correcta
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if(!result.Succeeded) return Unauthorized("Invalid username or password.");

                // Verificar si el usuario esta activo
                if(user.IsActive == 0) return Unauthorized("User is not active.");

                // Iniciar sesión
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Crear token
                var token = _tokenService.CreateToken(user);

                if (string.IsNullOrEmpty(token)) return Unauthorized("Invalid token.");

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = token
                    }
                );
                
            }catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para cerrar sesión en el sistema.
        /// </summary>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con el resultado de la operación.
        /// Retorna un mensaje que indica si se cerro sesión correctamente.
        /// </returns>
        [HttpPost("logout")]
        [Authorize(Roles = "User,Admin")]
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
    }
}