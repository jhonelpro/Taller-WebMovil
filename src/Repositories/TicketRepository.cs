using System.Data;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using api.src.Models.User;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace api.src.Repositories
{
    public class TicketRepository : ITicket
    {
        private readonly ApplicationDBContext _context;
        public TicketRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public Task<Ticket> CreateTicket(AppUser user)
        {
            new Ticket
            {
                Purchase_Date = DateTime.Now,
                UserId = user.Id,
                User = user,
            };
            
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTickets(string idUser)
        {
            throw new NotImplementedException();
        }
    }
}