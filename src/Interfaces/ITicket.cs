using api.src.DTOs.Purchase;
using api.src.Models;
using api.src.Models.User;

namespace api.src.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de tickets de compra, proporcionando métodos para la creación y obtención de tickets.
    /// </summary>
    public interface ITicket
    {
        /// <summary>
        /// Crea un nuevo ticket de compra basado en el usuario y los artículos de venta proporcionados.
        /// </summary>
        /// <param name="user">Parametro de tipo AppUser que representa el usuario que realiza la compra.</param>
        /// <param name="saleItems">Parametro de tipo List<SaleItem> que representa la lista de artículos vendidos asociados al ticket.</param>
        /// <returns>Instancia del ticket creado.</returns>
        Task<Ticket> CreateTicket(AppUser user, List<SaleItem> saleItems);

        /// <summary>
        /// Obtiene una lista de tickets asociados a un usuario específico.
        /// </summary>
        /// <param name="userId">Parametro de tipo string que representa el ID del usuario cuyos tickets se desean obtener.</param>
        /// <returns>Lista de objetos TicketDto que representan los tickets del usuario.</returns>
        Task<List<TicketDto>> GetTickets(string userId);
    }
}