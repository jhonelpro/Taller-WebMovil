using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Mappers;
using api.src.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
   
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Select(p => p.ToProductDto())
                .ToArrayAsync();

            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequestDto Product)
        {
            var product = Product.ToProductFromCreateDto();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductRequestDto product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            existingProduct.Name = string.IsNullOrWhiteSpace(product.Name) ? existingProduct.Name : product.Name;
            existingProduct.ProductTypeId = product.ProductTypeId != 0 ? product.ProductTypeId : existingProduct.ProductTypeId;
            existingProduct.Price = product.Price != 0 ? product.Price : existingProduct.Price;
            existingProduct.Stock = product.Stock != 0 ? product.Stock : existingProduct.Stock;
            existingProduct.Image = string.IsNullOrWhiteSpace(product.Image) ? existingProduct.Image : product.Image;
            await _context.SaveChangesAsync();
            return Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found");
            }
            
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }
    }
}