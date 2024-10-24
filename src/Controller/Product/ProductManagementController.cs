using api.src.DTOs;
using api.src.Helpers;
using api.src.Interfaces;
using api.src.Mappers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;
        
        public ProductManagementController(IProductRepository productRepository, Cloudinary cloudinary)
        {
            _productRepository = productRepository;
            _cloudinary = cloudinary;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] QueryObjectProduct query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var products = await _productRepository.GetProducts(query);
            return Ok(products);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductRequestDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (productDto.Image == null || productDto.Image.Length == 0)
            {
                return BadRequest("Image is required");
            }

            if (productDto.Image.ContentType != "image/jpeg" && productDto.Image.ContentType != "image/png")
            {
                return BadRequest("Image must be a jpeg or png file");
            }

            if (productDto.Image.Length > 2 * 1024 * 1024)
            {
                return BadRequest("Image must be less than 2MB");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(productDto.Image.FileName, productDto.Image.OpenReadStream()),
                Folder = "E-commerce-Products"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return BadRequest(uploadResult.Error.Message);
            }

            var product = productDto.ToProductFromCreateDto();
            var ProductAdded = await _productRepository.AddProduct(product, uploadResult);
            return Ok(ProductAdded.ToProductDto());
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductRequestDto product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (product.Image == null || product.Image.Length == 0)
            {
                return BadRequest("Image is required");
            }

            if (product.Image.ContentType != "image/jpeg" && product.Image.ContentType != "image/png")
            {
                return BadRequest("Image must be a jpeg or png file");
            }

            if (product.Image.Length > 2 * 1024 * 1024)
            {
                return BadRequest("Image must be less than 2MB");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(product.Image.FileName, product.Image.OpenReadStream()),
                Folder = "E-commerce-Products"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                return BadRequest(uploadResult.Error.Message);
            }

            var existingProduct = await _productRepository.UpdateProduct(id, product, uploadResult);
            
            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            return Ok(existingProduct.ToProductDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _productRepository.DeleteProduct(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }
            
            return Ok(product.ToProductDto());
        }
    }
}