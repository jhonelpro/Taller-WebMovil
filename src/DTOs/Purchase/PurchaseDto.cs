using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.Purchase
{
    public class PurchaseDto
    {

        public int PurchaseId { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime Transaction_Date { get; set; }

        public string Country { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Commune { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public double Purchase_TotalPrice { get; set; } = 0;

        public List<SaleItemDto> saleItemDtos { get; set; } = new List<SaleItemDto>();    
    }
}