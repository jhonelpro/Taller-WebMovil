using System.ComponentModel.DataAnnotations;
using api.src.Models;

namespace api.src.DTOs.Purchase
{
    public class CreatePurchaseDto
    {
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

        //EtityFramework relationship;
        public List<SaleItem> SaleItems { get; } = [];
    }
}