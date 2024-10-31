using System.Data;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using api.src.Models.User;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Win32.SafeHandles;

namespace api.src.Repositories
{
    public class TicketRepository : ITicket
    {
        private readonly ApplicationDBContext _context;
        public TicketRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Ticket> CreateTicket(AppUser user, List<SaleItem> saleItems)
        {
            Ticket ticket = new Ticket
            {
                Purchase_Date = DateTime.Now,
                UserId = user.Id,
                User = user,
                SaleItems = saleItems,
            };

            foreach (var saleItem in saleItems)
            {
                ticket.Purchase_TotalPrice += saleItem.TotalPrice;
            }
            
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public Task<List<Ticket>> GetTickets(string idUser)
        {
            throw new NotImplementedException();
        }
    }
}