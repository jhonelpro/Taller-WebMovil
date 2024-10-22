using api.src.DTOs.Purchase;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface ISaleItem
    {
        Task<List<SaleItem>> createSaleItem(List<ShoppingCartItem> shoppingCartItems, Purchase purchase);
        Task<List<PurchaseDto>> GetPurchasesAsync(string userId);
        Task<List<SaleItem>> getSaleItem(int purchaseId);
    }
}