using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; } = string.Empty;

        //EtityFramework relationship
        public int ProductTypesId { get; set; }
        public ProductType ProductType = null!;

        public List<Product_Cart> Product_Carts { get; set; } = [];
        public List<Product_Purchase> product_Purchases { get; set; } = [];
    }
}