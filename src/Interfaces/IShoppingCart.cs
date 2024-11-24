using api.src.Models;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión del carrito de compras, proporcionando métodos para crear y obtener el carrito de un usuario.
    /// </summary>
    public interface IShoppingCart
    {
        /// <summary>
        /// Crea un nuevo carrito de compras para un usuario específico.
        /// </summary>
        /// <param name="userId">Parametro de tipo int que representa el ID del usuario para el cual se va a crear el carrito de compras.</param>
        /// <returns>Instancia del carrito de compras creado para el usuario.</returns>
        Task<ShoppingCart> CreateShoppingCart(string userId);

        /// <summary>
        /// Obtiene el carrito de compras de un usuario específico.
        /// </summary>
        /// <param name="userId">Parametro de tipo string que representa el ID del usuario cuyo carrito de compras se desea obtener.</param>
        /// <returns>Carrito de compras del usuario, o null si no se encuentra.</returns>
        Task<ShoppingCart?> GetShoppingCart(string userId);
    }
}