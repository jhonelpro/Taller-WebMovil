using api.src.Models;
using api.src.Models.User;


namespace api.src.Interfaces
{
    public interface IPurchase
    {
        Task<Purchase> createPurchase(Purchase purchase, AppUser user);
        Task<byte[]> getPurchaseRecipt(int id);
        Task<Purchase> getPurchase(int id);
    }
}