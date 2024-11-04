using api.src.DTOs.Purchase;
using api.src.Helpers;
using api.src.Models;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de ítems de venta, proporcionando métodos para crear ítems de venta y recuperar compras.
    /// </summary>
    public interface ISaleItem
    {
        /// <summary>
        /// Crea ítems de venta a partir de una lista de artículos del carrito y una compra asociada.
        /// </summary>
        /// <param name="shoppingCartItems">Parametro de tipo List<ShoppingCartItem> que representa la lista de artículos del carrito de compras.</param>
        /// <param name="purchase">Parametro de tipo Purchase que representa la compra asociada.</param>
        /// <returns>Lista de ítems de venta creados.</returns>
        Task<List<SaleItem>> createSaleItem(List<ShoppingCartItem> shoppingCartItems, Purchase purchase);

        /// <summary>
        /// Obtiene una lista de compras asociadas a un usuario específico.
        /// </summary>
        /// <param name="userId">Parametro de tipo string que representa el ID del usuario cuyas compras se desean obtener.</param>
        /// <returns>Lista de compras del usuario representadas por objetos PurchaseDto.</returns>
        Task<List<PurchaseDto>> GetPurchasesAsync(string userId);

        /// <summary>
        /// Obtiene una lista de todas las compras disponibles para la vista del administrador.
        /// </summary>
        /// <returns>Lista de todas las compras representadas por objetos PurchaseDto.</returns>
        Task<List<PurchaseDto>> GetPurchasesAsyncForAdmin(QueryObjectSale queryObjectSale);

        /// <summary>
        /// Obtiene los ítems de venta asociados a una compra específica.
        /// </summary>
        /// <param name="purchaseId">Parametro de tipo int que representa el ID de la compra para la cual se desean obtener los ítems de venta.</param>
        /// <returns>Lista de ítems de venta asociados a la compra especificada.</returns>
        Task<List<SaleItem>> getSaleItem(int purchaseId);
    }
}