using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    /// <summary>
    /// Clase ChangePasswordDto que representa la solicitud para cambiar la contraseña de un usuario.
    /// Contiene las propiedades necesarias para validar la contraseña actual y las nuevas contraseñas.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Atributo de tipo string que representa la contraseña actual del usuario.
        /// Debe tener entre 8 y 20 caracteres y ser alfanumérica.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")] 
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string CurrentPassword { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa la nueva contraseña que se desea establecer.
        /// Debe tener entre 8 y 20 caracteres y ser alfanumérica.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")] 
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string NewPassword { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa la confirmación de la nueva contraseña.
        /// Debe tener entre 8 y 20 caracteres y ser alfanumérica.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")] 
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
