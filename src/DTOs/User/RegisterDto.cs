using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    /// <summary>
    /// Clase RegisterDto que representa la solicitud para registrar un nuevo usuario en la aplicación.
    /// Contiene las propiedades necesarias para almacenar información del usuario que se registrará.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el RUT del nuevo usuario. 
        /// Este campo es obligatorio para completar el registro.
        /// </summary>
        [Required]
        public string? Rut { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el nombre del nuevo usuario.
        /// Debe tener entre 8 y 255 caracteres de longitud.
        /// Debe tener solo caracteres del abecedario español.
        /// </summary>
        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "El nombre debe tener entre 8 y 255 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", ErrorMessage = "El nombre solo debe contener letras del alfabeto español.")]
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de nacimiento del nuevo usuario.
        /// Este campo es obligatorio para completar el registro.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el género del nuevo usuario.
        /// Debe ser uno de los siguientes valores: Femenino, Masculino, Prefiero no decirlo, Otro.
        /// </summary>
        [Required]
        [RegularExpression(@"Femenino|Masculino|Prefiero no decirlo|Otro", ErrorMessage = "El género debe ser uno de los valores especificados.")]
        public string? Gender { get; set; } = string.Empty!;

        /// <summary>
        /// Atributo de tipo string que representa la dirección de correo electrónico del nuevo usuario.
        /// Este campo es obligatorio y debe ser una dirección de correo válida.
        /// </summary>
        [Required]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la contraseña del nuevo usuario.
        /// Debe tener entre 8 y 20 caracteres de longitud y ser alfanumérica.
        /// </summary>
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica.")]
        public string? Password { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la confirmación de la contraseña del nuevo usuario.
        /// Este campo es obligatorio y debe coincidir con el valor de la contraseña.
        /// </summary>
        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}