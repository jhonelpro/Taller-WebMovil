using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.Purchase
{
    public class TicketDto
    {
        [Required]
        public DateTime Purchase_Date { get; set; }
        [Required]
        public double Purchase_TotalPrice { get; set; } = 0;
        [Required]
        public List<SaleItemDto> saleItemDtos { get; set; } = new List<SaleItemDto>(); 
    }
}