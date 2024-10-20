using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.DTOs.Product;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using api.src.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace api.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    //Agregar Authorize
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var products = await _productRepository.GetAvailableProducts(query);
                return Ok(products);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "Product not found")
                {
                    return NotFound(new { Message = ex.Message });
                }
                else if (ex.Message == "Product Type incorrect")
                {
                    return BadRequest(new { Message = ex.Message });
                }
                else
                {
                    return StatusCode(500, new { Message = "An error occurred while processing your request." });
                }
            }
        }
    }
}