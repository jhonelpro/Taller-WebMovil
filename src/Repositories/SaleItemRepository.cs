using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs.Purchase;
using api.src.Interfaces;
using api.src.Mappers;
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
                    Purchase = purchase,
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

        public async Task<List<PurchaseDto>> GetPurchasesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var purchases = await _context.Purchases
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync();

            if (purchases == null)
            {
                throw new ArgumentNullException(nameof(purchases));
            }

            var saleItems = await _context.SaleItems.Where(x => purchases.Select(y => y.Id).Contains(x.PurchaseId)).ToListAsync();

            if (saleItems == null)
            {
                throw new ArgumentNullException(nameof(saleItems));
            }

            var purchasesDtos = new List<PurchaseDto>();

            foreach (var purchase in purchases)
            {
                var saleItem = saleItems.Where(x => x.PurchaseId == purchase.Id).ToList();

                if (saleItem == null)
                {
                    throw new ArgumentNullException(nameof(saleItem));
                }

                var products = await _context.Products
                                               .Where(x => saleItem.Select(y => y.ProductId).Contains(x.Id))
                                               .Include(x => x.ProductType)
                                               .ToListAsync();

                if (products == null)
                {
                    throw new ArgumentNullException(nameof(products));
                }

                var purchaseDto = new PurchaseDto
                {
                    PurchaseId = purchase.Id,
                    Transaction_Date = purchase.Transaction_Date,
                    Country = purchase.Country,
                    City = purchase.City,
                    Commune = purchase.Commune,
                    Street = purchase.Street,
                    saleItemDtos = PurchaseMapper.ToSaleItemDto(saleItem, products)
                };

                purchasesDtos.Add(purchaseDto);
            }

            return purchasesDtos;
        }

        public async Task<List<PurchaseDto>> GetPurchasesAsyncForAdmin()
        {
            var purchases = await _context.Purchases.ToListAsync();

            if (purchases == null)
            {
                throw new ArgumentNullException(nameof(purchases));
            }

            var saleItems = await _context.SaleItems.Where(x => purchases.Select(y => y.Id).Contains(x.PurchaseId)).ToListAsync();

            if (saleItems == null)
            {
                throw new ArgumentNullException(nameof(saleItems));
            }

            var purchasesDtos = new List<PurchaseDto>();

            foreach (var purchase in purchases)
            {
                var saleItem = saleItems.Where(x => x.PurchaseId == purchase.Id).ToList();

                if (saleItem == null)
                {
                    throw new ArgumentNullException(nameof(saleItem));
                }

                var products = await _context.Products
                                               .Where(x => saleItem.Select(y => y.ProductId).Contains(x.Id))
                                               .Include(x => x.ProductType)
                                               .ToListAsync();

                if (products == null)
                {
                    throw new ArgumentNullException(nameof(products));
                }

                var purchaseDto = new PurchaseDto
                {
                    PurchaseId = purchase.Id,
                    Transaction_Date = purchase.Transaction_Date,
                    Country = purchase.Country,
                    City = purchase.City,
                    Commune = purchase.Commune,
                    Street = purchase.Street,
                    saleItemDtos = PurchaseMapper.ToSaleItemDto(saleItem, products)
                };

                purchasesDtos.Add(purchaseDto);
            }

            return purchasesDtos;
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