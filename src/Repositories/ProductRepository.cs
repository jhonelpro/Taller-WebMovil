using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;

        public ProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        public async Task<Product?> UpdateProduct(int id, UpdateProductRequestDto product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.Name = string.IsNullOrWhiteSpace(product.Name) ? existingProduct.Name : product.Name;
            existingProduct.ProductTypeId = product.ProductTypeId != 0 ? product.ProductTypeId : existingProduct.ProductTypeId;
            existingProduct.Price = product.Price != 0 ? product.Price : existingProduct.Price;
            existingProduct.Stock = product.Stock != 0 ? product.Stock : existingProduct.Stock;
            existingProduct.Image = string.IsNullOrWhiteSpace(product.Image) ? existingProduct.Image : product.Image;
            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}