using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Helpers;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetAvailableProducts(QueryObject query);
        Task<Product> AddProduct(Product product);
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto);
        Task<Product?> DeleteProduct(int id);
    }
}