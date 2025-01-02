using api.src.DTOs.User;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace api.src.Controller
{
    /// <summary>
    /// Controlador de la cuenta del usuario.
    /// </summary>
    /// <remarks>
    /// Este controlador contiene las acciones necesarias para la gestión de la cuenta del usuario.
    /// las acciones disponibles son:
    /// <list type="bullet">
    /// <item>Editar perfil</item>
    /// <item>Cambiar contraseña</item>
    /// </list>
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo UserManager<AppUser> para la gestión de usuarios.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Constructor de la clase AccountController que recibe un objeto de tipo UserManager<AppUser>.
        /// </summary>
        /// <param name="userManager">Parámetro de tipo UserManager<AppUser> que sirve para inicializar el atributo _userManager</param>
        public AccountController(UserManager<AppUser> userManager)
        {
            // Inicializaron del atributo _userManager
            _userManager = userManager;
        }

        /// <summary>
        /// Endpoint para editar el perfil del usuario.
        /// </summary>
        /// <param name="editProfileDto">Parámetro que representa los datos del usuario que se actualizaran</param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con un mensaje de éxito o error.
        /// </returns>
        [HttpPost("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileDto editProfileDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (editProfileDto.DateOfBirth >= DateTime.Now) return BadRequest(new { message = "La fecha de nacimiento debe ser pasada"});

                if ((DateTime.Now.Year - editProfileDto.DateOfBirth.Year) < 13) return BadRequest(new { message = "Debes tener al menos 13 años para registrarte"});

                var user = await _userManager.GetUserAsync(User);

                if (user == null) return Unauthorized(new { message = "Usuario no encontrado"});

                user.Name = editProfileDto.Name;
                user.DateOfBirth = editProfileDto.DateOfBirth;
                user.Gender = editProfileDto.Gender;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "Perfil actualizado exitosamente"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }

        /// <summary>
        /// Endpoint para cambiar la contraseña del usuario.
        /// </summary>
        /// <param name="changePasswordDto">Parámetro que representa la confirmación de la contraseña para validar el cambio de contraseña de un usuario.</param>
        /// <returns>
        /// Retorna un objeto de tipo IActionResult con un mensaje de éxito o error.
        /// </returns>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized(new { message = "Usaurio no encontrado"});

                var passwordVerification = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);

                if (!passwordVerification) return BadRequest(new { message = "La contraseña actual es incorrecta"});

                if (!string.Equals(changePasswordDto.NewPassword, changePasswordDto.ConfirmPassword, StringComparison.Ordinal)) return BadRequest(new { message = "Las contraseñas no coinciden"});

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(new { message = "Contraseña cambiada exitosamente"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }
    }
}