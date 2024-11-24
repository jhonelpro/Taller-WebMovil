using System;
using api.src.Models.User;

namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un ticket de compra, incluyendo detalles sobre la compra realizada y el usuario que la efectuó.
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único del ticket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de la compra asociada al ticket.
        /// </summary>
        public DateTime Purchase_Date { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio total de la compra asociada al ticket.
        /// </summary>
        public int Purchase_TotalPrice { get; set; } = 0;

        /// <summary>
        /// Atributo de tipo string que representa el identificador del usuario que realizó la compra.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo AppUser que representa el usuario que realizó la compra.
        /// </summary>
        public AppUser User { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo List<SaleItem> que contiene los elementos de venta asociados a este ticket.
        /// </summary>
        public List<SaleItem> SaleItems { get; set; } = [];
    }
}