using api.src.DTOs;
using api.src.DTOs.Product;
using api.src.Helpers;
using api.src.Models;
using CloudinaryDotNet.Actions;

namespace api.src.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetProductById(int id);
        Task<List<ProductDtoForAdmin>> GetProducts(QueryObjectProduct query);
        Task<List<ProductDto>> GetAvailableProducts(QueryObject query);
        Task<List<ProductDto>> GetProductsClient(QueryObjectProductClient query);
        Task<Product> AddProduct(Product product, ImageUploadResult uploadResult);
        Task<Product?> UpdateProduct(int id, UpdateProductRequestDto updateProductRequestDto, ImageUploadResult uploadResult);
        Task<Product?> DeleteProduct(int id);
    }
}