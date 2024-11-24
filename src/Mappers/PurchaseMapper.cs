using api.src.DTOs.Purchase;
using api.src.Models;

namespace api.src.Mappers
{
    /// <summary>
    /// Clase que proporciona métodos de mapeo entre entidades de compra y sus DTOs correspondientes.
    /// </summary>
    public static class PurchaseMapper
    {
        /// <summary>
        /// Convierte un objeto de tipo CreatePurchaseDto a un objeto de tipo Purchase.
        /// </summary>
        /// <param name="purchaseDto">Parametro de tipo CreatePurchaseDto que representa los datos necesarios para crear una nueva compra.</param>
        /// <returns>Un objeto de tipo Purchase que representa la nueva compra creada.</returns>
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

        /// <summary>
        /// Convierte un objeto de tipo Purchase a su representación DTO.
        /// </summary>
        /// <param name="purchase">Parametro de tipo Purchase que representa la compra a convertir.</param>
        /// <param name="saleItemDtos">Parametro de tipo List<SaleItemDto> que representa la lista de elementos de venta asociados a la compra.</param>
        /// <returns>Un objeto de tipo PurchaseDto que representa la compra convertida.</returns>
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

        /// <summary>
        /// Convierte una lista de objetos SaleItem a una lista de SaleItemDto.
        /// </summary>
        /// <param name="saleItems">Parametro de tipo List<SaleItem> que representa la lista de elementos de venta a convertir.</param>
        /// <param name="products">Parametro de tipo List<Product> que representa la lista de productos asociados a los elementos de venta.</param>
        /// <returns>Una lista de objetos de tipo SaleItemDto que representan los elementos de venta convertidos.</returns>
        /// <exception cref="ArgumentNullException">Se lanza si no se encuentra un producto asociado al elemento de venta.</exception>
        public static List<SaleItemDto> ToSaleItemDto(List<SaleItem> saleItems, List<Product> products)
        {
            // Crea una lista de elementos de tipo SaleItemDto.
            var saleItemDtos = new List<SaleItemDto>();

            foreach (var saleItem in saleItems)
            {
                // Busca el producto asociado al saleItem.
                var product = products.FirstOrDefault(x => x.Id == saleItem.ProductId);

                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product));
                }

                // Aniade un saleItemDto a la lista.
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