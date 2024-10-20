using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using api.src.Models.User;

namespace api.src.Repositories
{
    public class PurchaseRepository : IPurchase
    {
        private readonly ApplicationDBContext _context;

        public PurchaseRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Purchase> createPurchase(Purchase purchase, AppUser user)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }

            purchase.UserId = user.Id;
            purchase.Transaction_Date = DateTime.Now;

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            return purchase;
        }

        public async Task<Purchase> getPurchase(int id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }

            return purchase;
        }
    }
}