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
        /// <param name="product">Parámetro de tipo Product que representa el producto a convertir.</param>
        /// <returns>Un objeto de tipo ProductDto que representa el producto convertido.</returns>
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                ProductType = product.ProductType,
                ImageUrl = product.ImageUrl
            };
        }

        /// <summary>
        /// Convierte un objeto de tipo Product a su representación DTO para administradores.
        /// </summary>
        /// <param name="product">Parámetro de tipo Product que representa el producto a convertir.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Convierte un objeto de tipo CreateProductRequestDto a su representación de tipo Product.
        /// </summary>
        /// <param name="createProductRequestDto">Parámetro de tipo CreateProductRequestDto que representa el producto a convertir.</param>
        /// <returns></returns>
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
    }
}