using api.src.Models.User;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de tokens de autenticación, proporcionando un método para crear un token basado en un usuario.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Crea un token de autenticación para un usuario específico.
        /// </summary>
        /// <param name="user">Parametro de tipo AppUser que representa el usuario para el cual se genera el token.</param>
        /// <returns>Token de autenticación generado como una cadena de texto.</returns>
        string CreateToken(AppUser user);
    }
}