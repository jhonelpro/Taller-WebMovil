using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
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


        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.Include(p => p.ProductType).Select(p => p.ToProductDto()).ToList();

            return Ok(products);
        }
    }
}