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
using api.src.Repositories;

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
        /// Atributo de tipo IShoppingCartItem que se encarga de manejar las operaciones de los items del carrito de compras y asi inyectar dependencias.
        /// </summary>
        private readonly IShoppingCartItem _shoppingCartItem;

        /// <summary>
        /// Atributo de tipo ICookieService que se encarga de manejar las operaciones de las cookies.
        /// </summary>
        private readonly ICookieService _cookieService;

        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager,
        IShoppingCart shoppingCart, IShoppingCartItem shoppingCartItem, ICookieService cookieService)
        {
            // Inicializa el atributo _userManager con el valor del parámetro userManager
            _userManager = userManager;
            // Inicializa el atributo _tokenService con el valor del parámetro tokenService
            _tokenService = tokenService;
            // Inicializa el atributo _signInManager con el valor del parámetro signInManager
            _signInManager = signInManager;
            // Inicializa el atributo _shoppingCart con el valor del parámetro shoppingCart
            _shoppingCart = shoppingCart;
            // Inicializa el atributo _shoppingCartItem con el valor del parámetro shoppingCartItem
            _shoppingCartItem = shoppingCartItem;
            // Inicializa el atributo _cookieService con el valor del parámetro cookieService
            _cookieService = cookieService;
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

                if (await _userManager.Users.AnyAsync(p => p.Email == registerDto.Email)) return BadRequest( new { message = "El correo ya existe"});

                if (string.IsNullOrEmpty(registerDto.Rut)) return BadRequest("El rut es requerido");

                if (await _userManager.Users.AnyAsync(p => p.Rut == registerDto.Rut)) return BadRequest( new { message = "El rut ya existe"});

                if (!RutValidations.IsValidRut(registerDto.Rut)) return BadRequest( new { message = "El rut es invalido"});

                if (registerDto.DateOfBirth >= DateTime.Now) return BadRequest( new { message = "La fecha de nacimiento no puede ser mayor o igual a la fecha actual"});

                if ((DateTime.Now.Year - registerDto.DateOfBirth.Year) < 13) return BadRequest( new { message = "Debes ser mayor de 13 años para registrarte"});

                if (string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.ConfirmPassword)) return BadRequest( new { message = "La contraseñas son requeridas"});

                if (!string.Equals(registerDto.Password, registerDto.ConfirmPassword, StringComparison.Ordinal)) return BadRequest( new { message = "Contraseña y confirmación de contraseña no coinciden"});

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
                        var shoppingCart = await _shoppingCart.CreateShoppingCart(user.Id);

                        if (shoppingCart != null)
                        {
                            if (Request.Cookies.ContainsKey("ShoppingCart"))
                            {
                                var cartItems = _cookieService.GetCartItemsFromCookies();

                                await _shoppingCartItem.AddShoppingCarItem(cartItems, shoppingCart.Id);

                                if (cartItems.Count > 0)
                                {
                                    _cookieService.ClearCartItemsInCookie();
                                }
                            }
                        }

                        return Ok(new NewUserDto
                        {
                            UserName = user.UserName!,
                            Email = user.Email!,
                            Token = _tokenService.CreateToken(user)
                        });
                    }

                    else
                    {
                        return StatusCode(500, new { message = roleResult.Errors});
                    }
                }else
                {
                    return StatusCode(500,  new { message = createUser.Errors});
                }

            } 
            catch (Exception e)
            {
                return StatusCode(500,  new { message = e.Message});
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
                // Verificar si el usuario ya esta autenticado.
                if (User.Identity?.IsAuthenticated == true)
                {
                    return BadRequest(new { message = "Sesion activa" });
                }

                // Validar modelo.
                if(!ModelState.IsValid) return BadRequest(ModelState);

                // Verificar si el usuario existe
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if(user == null) return Unauthorized( new { message = "Correo o contraseña incorrectos."});

                // Verificar si la contraseña es correcta
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if(!result.Succeeded) return Unauthorized( new { message = "Correo o contraseña incorrectos."});

                // Verificar si el usuario esta activo
                if(user.IsActive == 0) return Unauthorized( new { message = "Usuario desactivado."});

                // Iniciar sesión
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Crear token
                var token = _tokenService.CreateToken(user);

                if (string.IsNullOrEmpty(token)) return Unauthorized( new { message = "Token invalido"});

                var shoppingCart = await _shoppingCart.GetShoppingCart(user.Id);

                if (shoppingCart != null)
                {
                    if (Request.Cookies.ContainsKey("ShoppingCart"))
                    {
                        var cartItems = _cookieService.GetCartItemsFromCookies();

                        await _shoppingCartItem.AddShoppingCarItem(cartItems, shoppingCart.Id);

                        if (cartItems.Count > 0)
                        {
                            _cookieService.ClearCartItemsInCookie();
                        }
                    }
                }

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = token
                    }
                );
                
            }catch (Exception ex) {
                return StatusCode(500,  new { message = ex.Message});
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
            // Verificar si el usuario no esta autenticado.
            if (User.Identity?.IsAuthenticated != true)
            {
                return BadRequest(new { message = "No se encontró ninguna sesión activa" });
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                await _tokenService.AddToBlacklistAsync(token);
                await _signInManager.SignOutAsync();
            }

            return Ok(new { message = "Cierre de sesión exitoso" });
        }
    }
}