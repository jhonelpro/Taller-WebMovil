using api.src.Models;
using api.src.Models.User;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de compras, proporcionando métodos para crear, obtener detalles de una compra y generar un recibo.
    /// </summary>
    public interface IPurchase
    {
        /// <summary>
        /// Crea una nueva compra para un usuario específico.
        /// </summary>
        /// <param name="purchase">Parametro Purchase que representa el objeto de compra a crear.</param>
        /// <param name="user">Parametro de tipo AppUser que representa el usuario asociado a la compra.</param>
        /// <returns>Objeto de la compra creada.</returns>
        Task<Purchase> createPurchase(Purchase purchase, AppUser user);

        /// <summary>
        /// Obtiene el recibo de una compra en formato de bytes.
        /// </summary>
        /// <param name="purchaseId">Parametro de tipo int que representa el ID de la compra.</param>
        /// <param name="userId">Parametro de tipo int que representa el ID del usuario asociado a la compra.</param>
        /// <returns>Recibo de la compra en formato byte array.</returns>
        Task<byte[]> getPurchaseRecipt(int purchaseId, string userId);

        /// <summary>
        /// Obtiene los detalles de una compra específica para un usuario.
        /// </summary>
        /// <param name="purchaseId">Parametro de tipo int que representa el ID de la compra.</param>
        /// <param name="userId">Parametro de tipo int que representa el ID del usuario asociado a la compra.</param>
        /// <returns>Objeto de la compra correspondiente o null si no se encuentra.</returns>
        Task<Purchase> getPurchase(int purchaseId, string userId);
    }
}