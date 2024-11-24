using System.Data;
using api.src.Data;
using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Models.User;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    /// <summary>
    /// Clase que implementa las operaciones de gestión de tickets de compra.
    /// Este repositorio interactúa con el contexto de la base de datos para realizar operaciones CRUD sobre los tickets.
    /// </summary>
    public class TicketRepository : ITicket
    {
        /// <summary>
        /// Contexto de la base de datos, utilizado para realizar consultas y operaciones sobre los datos.
        /// </summary>
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor que inicializa el repositorio con un contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de base de datos que se usará para realizar operaciones de acceso a datos.</param>
        public TicketRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crea un nuevo ticket de compra asociado a un usuario específico.
        /// Calcula el precio total de la compra sumando los precios de los items vendidos.
        /// </summary>
        /// <param name="user">Usuario que realiza la compra.</param>
        /// <param name="saleItems">Lista de items vendidos en la compra.</param>
        /// <returns>El objeto Ticket creado y agregado a la base de datos.</returns>
        public async Task<Ticket> CreateTicket(AppUser user, List<SaleItem> saleItems)
        {
            // Crear un nuevo objeto Ticket con la fecha de compra, ID del usuario y items de venta
            Ticket ticket = new Ticket
            {
                Purchase_Date = DateTime.Now, // Establecer la fecha de compra actual
                UserId = user.Id, // Asociar el ticket al usuario que realizó la compra
                User = user, // Almacenar el objeto usuario asociado
                SaleItems = saleItems, // Almacenar los items vendidos en el ticket
            };

            // Calcular el precio total de la compra sumando los precios de cada item vendido
            foreach (var saleItem in saleItems)
            {
                ticket.Purchase_TotalPrice += saleItem.TotalPrice; // Sumar el precio total del item al ticket
            }
            
            // Agregar el nuevo ticket al contexto y guardar los cambios en la base de datos
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Retornar el ticket creado
            return ticket;
        }

        /// <summary>
        /// Obtiene la lista de tickets de compra asociados a un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario cuyos tickets se desean obtener.</param>
        /// <returns>Lista de objetos TicketDto que representan los tickets del usuario.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si el ID del usuario es nulo o vacío.</exception>
        public async Task<List<TicketDto>> GetTickets(string userId)
        {
            // Validar que el ID del usuario no sea nulo ni vacío
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User id cannot be null or empty.");
            }

            // Obtener todos los tickets asociados al ID del usuario
            var tickets = await _context.Tickets
                                        .OrderBy(x => x.Purchase_Date) // Ordenar por fecha de compra 
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync();

            // Si no se encuentran tickets, lanzar una excepción
            if (tickets == null)
            {
                throw new ArgumentNullException(nameof(tickets), "No tickets found for the user.");
            }

            var ticketsDtos = new List<TicketDto>(); // Lista para almacenar los DTOs de los tickets

            // Iterar sobre cada ticket encontrado para crear su representación DTO
            foreach (var ticket in tickets)
            {
                // Obtener los items de venta asociados al ticket
                var saleItems = await _context.Tickets
                            .Where(x => x.UserId == userId)
                            .Where(x => x.Id == ticket.Id)
                            .Include(p => p.SaleItems) // Incluir los items de venta en la consulta
                            .SelectMany(t => t.SaleItems)
                            .ToListAsync();

                // Si no se encuentran items de venta, lanzar una excepción
                if (saleItems == null)
                {
                    throw new ArgumentNullException(nameof(saleItems), "No sale items found for the ticket.");
                }

                // Obtener los productos asociados a los items de venta
                var products = await _context.Products
                                               .Where(x => saleItems.Select(y => y.ProductId).Contains(x.Id))
                                               .Include(x => x.ProductType) // Incluir el tipo de producto
                                               .ToListAsync();

                // Si no se encuentran productos, lanzar una excepción
                if (products == null)
                {
                    throw new ArgumentNullException(nameof(products), "No products found for the sale items.");
                }

                // Mapear el ticket a un TicketDto
                var ticketDto = new TicketDto
                {
                    Purchase_Date = ticket.Purchase_Date,
                    Purchase_TotalPrice = ticket.Purchase_TotalPrice,
                    saleItemDtos = PurchaseMapper.ToSaleItemDto(saleItems, products) // Mapear los items de venta a DTOs
                };

                ticketsDtos.Add(ticketDto); // Agregar el DTO del ticket a la lista
            }

            // Retornar la lista de DTOs de tickets
            return ticketsDtos;
        }
    }
}