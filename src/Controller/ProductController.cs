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