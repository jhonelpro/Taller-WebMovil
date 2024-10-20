using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}