
using System;
using api.src.Models.User;

namespace api.src.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime Purchase_Date { get; set; }
        public double Purchase_TotalPrice { get; set; } = 0;
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
        public List<SaleItem> SaleItems { get; } = [];
    }
}