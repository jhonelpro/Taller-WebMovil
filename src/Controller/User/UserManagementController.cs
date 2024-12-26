using api.src.Helpers;
using api.src.Mappers;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    /// <summary>
    /// Controlador para la gestión de usuarios.
    /// </summary>
    /// <remarks>
    /// En este controlador se encuentran los métodos necesarios para la gestión de usuarios.
    /// Las operaciones que se pueden realizar son: obtener usuarios y cambiar el estado de un usuario.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        /// <summary>
        /// Atributo para la gestión de usuarios.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Constructor de la clase UserManagementController que recibe un objeto de tipo UserManager<AppUser>.
        /// </summary>
        /// <param name="userManager">Parámetro que sirve para la inicializar el atributo _userManager</param>
        public UserManagementController(UserManager<AppUser> userManager)
        {
            // Inicializa del atributo _userManager
            _userManager = userManager;
        }

        /// <summary>
        /// Endpoint para obtener los usuarios.
        /// </summary>
        /// <param name="query">Parámetro que permite realizar una solicitud que muestre los usuarios con un nombre en especifico</param>
        /// <returns>
        /// Retorna una lista con todos los usuarios de la base de datos que cumplan con los criterios de la query
        /// </returns>
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] QueryObjectUsers query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _userManager.Users.AsQueryable();

                // Si el query no es nulo, se filtra por el nombre del usuario
                if (!string.IsNullOrEmpty(query.Name))
                {
                    string nameToSearch = query.Name.ToUpper();
                    user = user.Where(p => p.Name != null && p.Name.ToUpper().Contains(nameToSearch));
                }

                // Se obtiene la lista de usuarios de forma paginada
                var usersList = await user.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

                // Se mapea la lista de usuarios a una lista de UserDto
                var usersListDto = usersList.Select(u => UserMapper.MapUserToUserDto(u)).ToList();
                
                return Ok(usersListDto);                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }

        /// <summary>
        /// Endpoint para cambiar el estado de un usuario.
        /// </summary>
        /// <param name="email">Parámetro que representa el email del usuario al cual se quiere deshabilitar o habilitar</param>
        /// <returns></returns>
        [HttpPost("ChangeStateUser/{email}")]
        public async Task<IActionResult> ChangeStateUser(string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(email)) return BadRequest(new { message = "Email is required."});

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return BadRequest(new { message = "User not found."});
                }

                // Se cambia el estado del usuario dependiendo de su estado actual
                if (user.IsActive == 1){
                    user.IsActive = 0;
                } else {
                    user.IsActive = 1;
                }

                // Se actualiza el usuario
                await _userManager.UpdateAsync(user);

                // Se mapea el usuario a un UserDto
                return Ok(UserMapper.MapUserToUserDto(user));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}