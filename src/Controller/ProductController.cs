using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToArrayAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No product found.");
            }

            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("keyword/{keyword}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsBykeyWorkd(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)){
                return BadRequest("Keyword cannot be empty or null.");
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No product found");
            }

            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [HttpGet("Type/{Type}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByType(int Type)
        {
            var products = await _context.Products
                .Where(p => p.ProductTypesId == Type)
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No product found");
            }

            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Ascending")]
        public async Task<ActionResult<IEnumerable<Product>>> AscendingForm()
        {
            var products = await _context.Products.OrderBy(p => p.Price).ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No product found");
            }

            return Ok(products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Descending")]
        public async Task<ActionResult<IEnumerable<Product>>> DescendingForm()
        {
            var Products = await _context.Products.OrderBy(p => p.Price).Reverse().ToListAsync();

            if (Products == null || !Products.Any())
            {
                return NotFound("No product found");
            }

            return Ok(Products);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {

            if (_context.Products.Any(u => u.Name == product.Name && u.ProductTypesId == product.ProductTypesId))
            {
                return BadRequest(new { message = "Este producto ya existe."});
            }

            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult UpdateProduct([FromRoute] int id, [FromBody] Product product)
        {
            var productToUpdate = _context.Products.FirstOrDefault(p => p.Id == id);
            
            if (productToUpdate == null)
            {
                return NotFound("No se encontro el producto");
            }

            productToUpdate.Name = product.Name;
            productToUpdate.Price = product.Price;
            productToUpdate.Stock = product.Stock;
            productToUpdate.Image = product.Image;
            productToUpdate.ProductTypesId = product.ProductTypesId;

            _context.SaveChanges();
            return Ok(productToUpdate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult DeleteProduct([FromRoute] int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(product);
        }
    }
}