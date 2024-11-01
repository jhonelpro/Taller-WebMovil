using api.src.DTOs.Purchase;
using api.src.Models;

namespace api.src.Mappers
{
    public static class PurchaseMapper
    {
        public static Purchase ToPurchaseFromCreateDto(this CreatePurchaseDto purchaseDto)
        {
            return new Purchase
            {
                Country = purchaseDto.Country,
                City = purchaseDto.City,
                Commune = purchaseDto.Commune,
                Street = purchaseDto.Street,
            };
        }

        public static PurchaseDto ToPurchaseDto(this Purchase purchase, List<SaleItemDto> saleItemDtos)
        {
            return new PurchaseDto
            {
                PurchaseId = purchase.Id,
                Transaction_Date = purchase.Transaction_Date,
                Country = purchase.Country,
                City = purchase.City,
                Commune = purchase.Commune,
                Street = purchase.Street,
                saleItemDtos = saleItemDtos
            };
        }

        public static List<SaleItemDto> ToSaleItemDto(List<SaleItem> saleItems, List<Product> products)
        {
            var saleItemDtos = new List<SaleItemDto>();

            foreach (var saleItem in saleItems)
            {
                var product = products.FirstOrDefault(x => x.Id == saleItem.ProductId);

                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                saleItemDtos.Add(new SaleItemDto
                {
                    ProductName = product.Name,
                    ProductType = product.ProductType.Name,
                    Quantity = saleItem.Quantity,
                    UnitPrice = saleItem.UnitPrice,
                    TotalPrice = saleItem.TotalPrice
                });
            }

            return saleItemDtos;
        }

        public static List<SaleItemDto> ToSaleItemDtoTicket(List<SaleItem> saleItems, List<Product> products)
        {
            var saleItemDtos = new List<SaleItemDto>();

            foreach (var saleItem in saleItems)
            {
                var product = products.FirstOrDefault(x => x.Id == saleItem.ProductId);

                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                saleItemDtos.Add(new SaleItemDto
                {
                    ProductName = product.Name,
                    ProductType = product.ProductType.Name,
                    Quantity = saleItem.Quantity,
                    UnitPrice = saleItem.UnitPrice,
                    TotalPrice = saleItem.TotalPrice
                });
            }

            return saleItemDtos;
        }
    }
}