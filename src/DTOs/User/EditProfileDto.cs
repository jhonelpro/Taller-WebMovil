using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    /// <summary>
    /// Clase EditProfileDto que representa la solicitud para editar el perfil de un usuario.
    /// Contiene las propiedades necesarias para actualizar la información personal del usuario.
    /// </summary>
    public class EditProfileDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del usuario. 
        /// Debe tener entre 8 y 255 caracteres.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "El nombre debe tener entre 8 y 255 caracteres.")]
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de nacimiento del usuario. 
        /// Este campo es obligatorio.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el género del usuario. 
        /// Debe ser uno de los siguientes valores: "Femenino", "Masculino", "Prefiero no decirlo", "Otro".
        /// </summary>
        [Required]
        [RegularExpression(@"Femenino|Masculino|Prefiero no decirlo|Otro", ErrorMessage = "El género debe ser uno de los valores especificados.")]
        public string? Gender { get; set; } = string.Empty;
    }
}