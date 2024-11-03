using api.src.Models;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de los elementos del carrito de compras, proporcionando métodos para la creación, actualización, eliminación y obtención de los artículos del carrito.
    /// </summary>
    public interface IShoppingCartItem
    {
        /// <summary>
        /// Crea un nuevo elemento en el carrito de compras.
        /// </summary>
        /// <param name="productId">Parametro de tipo int que representa el ID del producto que se va a añadir al carrito.</param>
        /// <param name="cartId">Parametro de tipo int que representa el ID del carrito en el que se añadirá el producto.</param>
        /// <param name="quantity">Parametro de tipo int que representa la cantidad del producto a añadir.</param>
        /// <returns>Instancia del elemento del carrito de compras creado.</returns>
        Task<ShoppingCartItem> CreateShoppingCartItem(int productId, int cartId, int quantity);

        /// <summary>
        /// Añade una lista de elementos al carrito de compras existente.
        /// </summary>
        /// <param name="cartItems">Parametro de tipo List<ShoppingCartItem> que representa la lista de elementos del carrito que se desean añadir.</param>
        /// <param name="cartId">Parametro de tipo int que representa el ID del carrito al que se añadirán los elementos.</param>
        /// <returns>Instancia del elemento del carrito de compras añadido.</returns>
        Task<ShoppingCartItem> AddShoppingCarItem(List<ShoppingCartItem> cartItems, int cartId);

        /// <summary>
        /// Añade un nuevo artículo al carrito de compras.
        /// </summary>
        /// <param name="productId">Parametro de tipo int que representa el ID del producto que se va a añadir.</param>
        /// <param name="cartId">Parametro de tipo int que representa el ID del carrito en el que se añadirá el producto.</param>
        /// <param name="quantity">Parametro de tipo int que representa la cantidad del producto a añadir.</param>
        /// <returns>Instancia del nuevo elemento del carrito de compras.</returns>
        Task<ShoppingCartItem> AddNewShoppingCartItem(int productId, int cartId, int quantity);

        /// <summary>
        /// Actualiza la cantidad de un elemento en el carrito de compras.
        /// </summary>
        /// <param name="productId">Parametro de tipo int que representa el ID del producto cuyo artículo se va a actualizar.</param>
        /// <param name="quantity">Parametro de tipo int que representa la nueva cantidad del producto.</param>
        /// <param name="isIncrement">Parametro de tipo bool? que indica si la cantidad debe incrementarse (true) o decrementarse (false).</param>
        /// <returns>Instancia del elemento del carrito de compras actualizado.</returns>
        Task<ShoppingCartItem> UpdateShoppingCartItem(int productId, int quantity, bool? isIncrement);

        /// <summary>
        /// Elimina un elemento del carrito de compras.
        /// </summary>
        /// <param name="productId">Parametro de tipo int que representa el ID del producto que se va a eliminar del carrito.</param>
        /// <returns>Instancia del elemento del carrito de compras con el elemento eliminado.</returns>
        Task<ShoppingCartItem> RemoveShoppingCartItem(int productId);

        /// <summary>
        /// Obtiene todos los elementos del carrito de compras especificado.
        /// </summary>
        /// <param name="cartId">Parametro de tipo int que representa el ID del carrito cuyos elementos se desean obtener.</param>
        /// <returns>Lista de elementos del carrito de compras.</returns>
        Task<List<ShoppingCartItem>> GetShoppingCartItems(int cartId);

        /// <summary>
        /// Obtiene un elemento específico del carrito de compras basado en el ID del producto.
        /// </summary>
        /// <param name="productId">Parametro de tipo int que representa el ID del producto cuyo artículo se desea obtener.</param>
        /// <returns>Instancia del elemento del carrito de compras, o null si no se encuentra.</returns>
        Task<ShoppingCartItem> GetShoppingCartItem(int productId);
    }
}