using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.Product
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductManagementController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        
        public ProductManagementController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] QueryObjectProduct query)
        {
            var products = await _productRepository.GetProducts(query);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductRequestDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = productDto.ToProductFromCreateDto();
            await _productRepository.AddProduct(product);
            return Ok(product.ToProductDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductRequestDto product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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