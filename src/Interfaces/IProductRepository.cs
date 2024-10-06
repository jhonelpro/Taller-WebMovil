using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetProducts();
        Task<List<ProductDto>> GetAvailableProducts(string? textFilter = null, string? productType = null, string? sortByPrice = null, int pageNumber = 1, int pageSize = 10);
        Task<Product> AddProduct(Product product);
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto);
        Task<Product?> DeleteProduct(int id);
    }
}