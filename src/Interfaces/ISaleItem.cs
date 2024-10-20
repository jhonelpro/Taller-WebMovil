using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface ISaleItem
    {
        Task<List<SaleItem>> createSaleItem(List<ShoppingCartItem> shoppingCartItems, Purchase purchase);
        Task<List<SaleItem>> getSaleItem(int purchaseId);
    }
}