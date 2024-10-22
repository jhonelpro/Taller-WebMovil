using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.Purchase
{
    public class PurchaseDto
    {
        [Required]
        public int PurchaseId { get; set; }
        [Required]
        public DateTime Transaction_Date { get; set; }
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Commune { get; set; } = string.Empty;
        [Required]
        public string Street { get; set; } = string.Empty;
        [Required]
        public List<SaleItemDto> saleItemDtos { get; set; } = new List<SaleItemDto>();    
    }
}