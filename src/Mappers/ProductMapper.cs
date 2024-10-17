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
                ImageUrl = product.ImageUrl
            };
        }

        public static Product ToProductFromCreateDto(this CreateProductRequestDto createProductRequestDto)
        {
            return new Product
            {
                Name = createProductRequestDto.Name,
                Price = createProductRequestDto.Price,
                Stock = createProductRequestDto.Stock,
                ProductTypeId = createProductRequestDto.ProductTypeId,
            };
        }

        public static Product ToProductFromUpdateDto(this UpdateProductRequestDto updateProductRequestDto)
        {
            return new Product
            {
                Name = updateProductRequestDto.Name,
                Price = updateProductRequestDto.Price,
                Stock = updateProductRequestDto.Stock,
                ProductTypeId = updateProductRequestDto.ProductTypeId,
            };
        }
    }
}