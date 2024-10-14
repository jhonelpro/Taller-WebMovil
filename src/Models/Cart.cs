using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models.User;

namespace api.src.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public DateTime Create_Date { get; set; }

        //EtityFramework relationship
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public List<Product_Cart> Product_Carts { get; set; } = [];
    }
}