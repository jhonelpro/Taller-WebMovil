using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<List<ProductDto>> GetProducts(QueryObjectProduct query)
        {
            var products = _context.Products.Include(p => p.ProductType).AsQueryable();

            if (!string.IsNullOrEmpty(query.textFilter))
            {
                products = products.Where(p => p.Name.Contains(query.textFilter) ||
                                               p.ProductType.Name.Contains(query.textFilter) ||
                                               p.Price.ToString().Contains(query.textFilter) ||
                                               p.Stock.ToString().Contains(query.textFilter));
            }

            var skipNumber = (query.pageNumber - 1) * query.pageSize;

            return await products.Skip(skipNumber).Take(query.pageSize)
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        public async Task<Product> AddProduct(Product product, ImageUploadResult uploadResult)
        {
            if (product == null || uploadResult == null) throw new ArgumentNullException("Product or UploadResult cannot be null");
            
            var newProduct = new Product
            {
                Name = product.Name,
                ProductTypeId = product.ProductTypeId,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            await _context.Entry(newProduct).Reference(p => p.ProductType).LoadAsync();

            return newProduct;
        }

        public async Task<Product?> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<ProductDto>> GetAvailableProducts(QueryObject query)
        {
            var products = _context.Products.Include(p => p.ProductType).AsQueryable();

            if (!string.IsNullOrEmpty(query.textFilter))
            {
                products = products.Where(p => p.Name.Contains(query.textFilter) ||
                                               p.ProductType.Name.Contains(query.textFilter) ||
                                               p.Price.ToString().Contains(query.textFilter) ||
                                               p.Stock.ToString().Contains(query.textFilter));
                
                if(!products.Any())
                {
                    throw new Exception("Product not found");
                }
            }

            if (!string.IsNullOrEmpty(query.productType))
            {
                var validProductTypes = new[] { "Poleras", "Gorros", "Juguetería", "Alimentación", "Libros" };
                if (!validProductTypes.Contains(query.productType))
                {
                    throw new Exception("Product Type incorrect");
                }
                products = products.Where(p => p.ProductType.Name.ToLower() == query.productType.ToLower());

                if(!products.Any())
                {
                    throw new Exception("Product not found");
                }
            }

            if(!string.IsNullOrWhiteSpace(query.sortByPrice))
            {
                if(query.sortByPrice.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ? products.OrderByDescending(x => x.Price) : products.OrderBy(x => x.Price);
                }
            }

            var skipNumber = (query.pageNumber - 1) * query.pageSize;

            return await products.Skip(skipNumber).Take(query.pageSize)
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToListAsync();
        }

        public async Task<Product?> UpdateProduct(int id, UpdateProductRequestDto product, ImageUploadResult uploadResult)
        {
            var existingProduct = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingProduct == null)
            {
                throw new Exception("Product not found");
            }

            existingProduct.Name = product.Name;
            existingProduct.ProductTypeId = product.ProductTypeId;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            
            await _context.SaveChangesAsync();

            var productUpdated = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);
            return productUpdated;
        }

        public async Task<Product?> GetProductById(int id)
        {
            var product = await _context.Products.Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }
            
            return product;
        }
    }
}