using System.Data;
using api.src.Data;
using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<TicketDto>> GetTickets(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var tickets = await _context.Tickets
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync();

            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets));
            }

            var ticketsDtos = new List<TicketDto>();

            foreach (var ticket in tickets)
            {
                var saleItems = await _context.Tickets
                            .Where(x => x.UserId == userId)
                            .Where(x => x.Id == ticket.Id)
                            .Include(p => p.SaleItems)
                            .SelectMany(t => t.SaleItems)
                            .ToListAsync();

                if (saleItems == null)
                {
                    throw new ArgumentNullException(nameof(saleItems));
                }

                var products = await _context.Products
                                               .Where(x => saleItems.Select(y => y.ProductId).Contains(x.Id))
                                               .Include(x => x.ProductType)
                                               .ToListAsync();

                if (products == null)
                {
                    throw new ArgumentNullException(nameof(products));
                }

                var ticketDto = new TicketDto
                {
                    Purchase_Date = ticket.Purchase_Date,
                    Purchase_TotalPrice = ticket.Purchase_TotalPrice,
                    saleItemDtos = PurchaseMapper.ToSaleItemDtoTicket(saleItems, products)
                };

                ticketsDtos.Add(ticketDto);
            }

            return ticketsDtos;
        }
    }
}