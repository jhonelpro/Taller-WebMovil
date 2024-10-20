using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Models.User;

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