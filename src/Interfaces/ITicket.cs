using api.src.Models;
using api.src.Models.User;

namespace api.src.Interfaces
{
    public interface ITicket
    {
        Task<Ticket> CreateTicket(AppUser user, List<SaleItem> saleItems);
        Task<List<Ticket>> GetTickets(string userId);
    }
}