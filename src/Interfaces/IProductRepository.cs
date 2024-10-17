using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Helpers;
using api.src.Models;
using CloudinaryDotNet.Actions;

namespace api.src.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> GetProducts(QueryObjectProduct query);
        Task<List<ProductDto>> GetAvailableProducts(QueryObject query);
        Task<Product> AddProduct(Product product, ImageUploadResult uploadResult);
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto, ImageUploadResult uploadResult);
        Task<Product?> DeleteProduct(int id);
    }
}