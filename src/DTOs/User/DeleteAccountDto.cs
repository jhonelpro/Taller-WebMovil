using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    /// <summary>
    /// Clase DeleteAccountDto que representa la solicitud para eliminar la cuenta de un usuario.
    /// Contiene las propiedades necesarias para validar la contraseña y confirmar la eliminación de la cuenta.
    /// </summary>
    public class DeleteAccountDto
    {
        /// <summary>
        /// Atributo de tipo string que representa la contraseña del usuario. 
        /// Debe tener entre 8 y 20 caracteres y ser alfanumérica.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")] 
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo bool que indica si se confirma la eliminación de la cuenta.
        /// El valor predeterminado es false.
        /// </summary>
        public bool ConfirmDeleteAccount { get; set; } = false;
    }
}