using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    public class SaleItemRepository : ISaleItem
    {
        private readonly ApplicationDBContext _context;

        public SaleItemRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<List<SaleItem>> createSaleItem(List<ShoppingCartItem> shoppingCartItems, Purchase purchase)
        {
            if (shoppingCartItems == null)
            {
                throw new ArgumentNullException(nameof(shoppingCartItems));
            }

            if (purchase == null)
            {
                throw new ArgumentNullException(nameof(purchase));
            }

            var saleItems = new List<SaleItem>();

            foreach (var item in shoppingCartItems)
            {
                
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == item.ProductId);

                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                var saleItem = new SaleItem
                {
                    PurchaseId = purchase.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * item.Quantity,
                    ProductId = item.ProductId,
                    Product = product
                };

                saleItems.Add(saleItem);
                await _context.SaleItems.AddAsync(saleItem);
            }

            await _context.SaveChangesAsync();
            return saleItems;
        }

        public async Task<List<SaleItem>> getSaleItem(int purchaseId)
        {
            if (purchaseId == 0)
            {
                throw new ArgumentNullException(nameof(purchaseId));
            }

            var saleItem = await _context.SaleItems.Where(x => x.PurchaseId == purchaseId).ToListAsync();

            if (saleItem == null)
            {
                throw new ArgumentNullException(nameof(saleItem));
            }

            return saleItem;
        }
    }
}