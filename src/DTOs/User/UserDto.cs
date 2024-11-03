namespace api.src.DTOs.User
{
    /// <summary>
    /// Clase UserDto que representa un usuario dentro del sistema.
    /// Contiene las propiedades que describen al usuario, incluyendo sus datos personales y estado.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre de usuario.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa el RUT del usuario.
        /// </summary>
        public string? Rut { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el nombre completo del usuario.
        /// </summary>
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de nacimiento del usuario.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el género del usuario.
        /// </summary>
        public string? Gender { get; set; } = string.Empty!;

        /// <summary>
        /// Atributo de tipo string que representa la dirección de correo electrónico del usuario.
        /// </summary>
        public string? Email { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que indica si el usuario está activo.
        /// Un valor de 1 indica que el usuario está activo, mientras que 0 indica que está inactivo.
        /// </summary>
        public int IsActive { get; set; }
    }
}