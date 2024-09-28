using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Models;
using Bogus.DataSets;

namespace api.src.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                ProductType = product.ProductType,
                Image = product.Image
            };
        }
    }
}