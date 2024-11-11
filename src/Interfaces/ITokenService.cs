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
        /// <param name="user">Parámetro de tipo AppUser que representa el usuario para el cual se genera el token.</param>
        /// <returns>Token de autenticación generado como una cadena de texto.</returns>
        string CreateToken(AppUser user);

        /// <summary>
        /// Agrega un token a la lista negra de tokens.
        /// </summary>
        /// <param name="token">Parámetro que representa el token el cual se quiere agregar a la blacklist.</param>
        Task AddToBlacklistAsync(string token);

        /// <summary>
        /// Verifica si un token se encuentra en la blacklist de tokens.
        /// </summary>
        /// <param name="token">Parámetro que representa el token el cual se verificara si esta en la blacklist.</param>
        /// <returns>
        /// Retorna un valor booleano que indica si el token se encuentra en la blacklist.
        /// </returns>
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}