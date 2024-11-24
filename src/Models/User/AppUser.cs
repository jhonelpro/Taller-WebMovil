using Microsoft.AspNetCore.Identity;

namespace api.src.Models.User
{
    /// <summary>
    /// Clase que representa un usuario en el sistema, heredando de IdentityUser y añadiendo propiedades adicionales.
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// Atributo de tipo string que representa el RUT del usuario.
        /// </summary>
        public string? Rut { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el nombre del usuario.
        /// </summary>
        public string? Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de nacimiento del usuario.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el género del usuario.
        /// </summary>
        public string? Gender { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que indica si el usuario está activo (1) o no (0).
        /// </summary>
        public int IsActive { get; set; }
    }
}