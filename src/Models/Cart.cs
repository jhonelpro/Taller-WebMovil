using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime Create_Date { get; set; }

        //EtityFramework relationship
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<Product_Cart> Product_Carts { get; set; } = [];
    }
}