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
using api.src.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts(QueryObject query)
        {
            var products = await _productRepository.GetAvailableProducts(query);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequestDto productDto)
        {
            var product = productDto.ToProductFromCreateDto();
            await _productRepository.AddProduct(product);
            return Ok(product.ToProductDto());
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
            var existingProduct = await _productRepository.UpdateProduct(id, product);
            
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            return Ok(existingProduct.ToProductDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _productRepository.DeleteProduct(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }
            
            return Ok(product.ToProductDto());
        }
    }
}