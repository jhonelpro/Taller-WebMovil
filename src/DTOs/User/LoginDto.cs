using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.Auth
{
    /// <summary>
    /// Clase LoginDto que representa la solicitud de inicio de sesión de un usuario.
    /// Contiene las propiedades necesarias para autenticar al usuario en la aplicación.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre de usuario del usuario.
        /// Este campo es obligatorio.
        /// </summary>
        [Required]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa la contraseña del usuario.
        /// Debe tener entre 8 y 20 caracteres y ser alfanumérica.
        /// Este campo es obligatorio.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string Password { get; set; } = null!;
    }
}