using api.src.DTOs;
using api.src.DTOs.Product;
using api.src.Models;

namespace api.src.Mappers
{
    /// <summary>
    /// Clase que proporciona métodos de mapeo entre entidades de producto y sus DTOs correspondientes.
    /// </summary>
    public static class ProductMapper
    {
        /// <summary>
        /// Convierte un objeto de tipo Product a su representación DTO.
        /// </summary>
        /// <param name="product">Parametro de tipo Product que representa el producto a convertir.</param>
        /// <returns>Un objeto de tipo ProductDto que representa el producto convertido.</returns>
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

        public static ProductDtoForAdmin ToProductDtoForAdmin(this Product product)
        {
            return new ProductDtoForAdmin
            {
                Id = product.Id,
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

        /// <summary>
        /// Convierte un objeto de tipo UpdateProductRequestDto a un objeto de tipo Product.
        /// </summary>
        /// <param name="updateProductRequestDto">Parametro de tipo UpdateProductRequestDto que representa los datos necesarios para actualizar un producto existente.</param>
        /// <returns>Un objeto de tipo Product que representa el producto actualizado.</returns>
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

        public static UpdateProductRequestDto toUpdateProduct(this CreateProductRequestDto createProductRequestDto)
        {
            return new UpdateProductRequestDto
            {
                Name = createProductRequestDto.Name,
                Price = createProductRequestDto.Price,
                Stock = createProductRequestDto.Stock,
                ProductTypeId = createProductRequestDto.ProductTypeId,
                Image = createProductRequestDto.Image
            };
        }
    }
}