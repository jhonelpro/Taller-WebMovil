using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime Transaction_Date { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Commune { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;

        //EtityFramework relationship
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public List<Product_Purchase> product_Purchases { get; set; } = [];
    }
}