using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IShoppingCart_ProductInterface
    {
        Task<Product_Cart> addProductToCart(int cartId, int productId, int quantity);
        Task<Product_Cart> getProductCartById(int cartId, int productId);
    }
}