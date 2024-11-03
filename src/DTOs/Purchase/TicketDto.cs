using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.Purchase
{
    /// <summary>
    /// Clase TicketDto que representa un ticket de compra, con los datos a mostrar al usuario.
    /// </summary>
    public class TicketDto
    {
        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha en que se realizó la compra.
        /// </summary>
        [Required]
        public DateTime Purchase_Date { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio total de la compra.
        /// </summary>
        [Required]
        public double Purchase_TotalPrice { get; set; } = 0;

        /// <summary>
        /// Atributo de tipo List<SaleItemDto> que contiene los artículos de venta asociados a este ticket.
        /// </summary>
        [Required]
        public List<SaleItemDto> saleItemDtos { get; set; } = new List<SaleItemDto>(); 
    }
}