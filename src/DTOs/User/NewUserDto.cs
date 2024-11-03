namespace api.src.DTOs.Auth
{
    /// <summary>
    /// Clase NewUserDto que representa un nuevo usuario en la aplicación.
    /// Contiene las propiedades necesarias para almacenar información sobre el usuario recién creado.
    /// </summary>
    public class NewUserDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre de usuario del nuevo usuario.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa la dirección de correo electrónico del nuevo usuario.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo string que representa el token de autenticación asociado al nuevo usuario.
        /// Este token puede ser utilizado para validar la sesión del usuario.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}