using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IProductRepositorie
    {
        Task<List<Product>> GetProducts();
        Task<Product> AddProduct(CreateProductRequestDto createProductRequestDto);
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto);
        Task<Product?> DeleteProduct(int id);
    }
}