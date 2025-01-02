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
    /// <summary>
    /// Controlador de productos que maneja los endpoints asociados a la administración de productos.
    /// </summary>
    /// <remarks>
    /// Solo los usuarios con el rol de Admin pueden acceder a los endpoints de este controlador.
    /// El controlador permite realizar las siguientes acciones: 
    /// <list type="bullet">
    /// <item>Obtener todos los productos disponibles en la base de datos.</item>
    /// <item>Agregar un nuevo producto a la base de datos.</item>
    /// <item>Actualizar un producto en la base de datos.</item>
    /// <item>Eliminar un producto de la base de datos.</item>
    /// </list>
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductManagementController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo IProductRepository que permite la inyección de dependencias.
        /// </summary>
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Atributo de tipo Cloudinary que permite utilizar las opciones que ofrece Cloudinary para la gestión de imágenes de los productos .
        /// </summary>
        private readonly Cloudinary _cloudinary;
        
        /// <summary>
        /// Constructor de la clase ProductManagementController que recibe un objeto de tipo IProductRepository y un objeto de tipo Cloudinary.
        /// </summary>
        /// <param name="productRepository">Parámetro de tipo IProductRepository que sirve para inicializar el atributo _productRepository</param>
        /// <param name="cloudinary">Parámetro de tipo Cloudinary que sirve para inicializar el atributo _cloudinary</param>
        public ProductManagementController(IProductRepository productRepository, Cloudinary cloudinary)
        {
            // Inicializa el atributo _productRepository con el objeto de tipo IProductRepository recibido.
            _productRepository = productRepository;
            // Inicializa el atributo _cloudinary con el objeto de tipo Cloudinary recibido.
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Endpoint que permite obtener todos los productos disponibles en la base de datos.
        /// </summary>
        /// <param name="query">Parámetro que permite realizar una solicitud que muestre productos con una característica especifica</param>
        /// <returns>
        /// Retorna una lista con todos los productos de la base de datos que cumplan con los criterios de la query
        /// <list type="bullet">
        /// <item>200 OK con la lista de productos que coinciden con los criterios de la consulta.</item>
        /// <item>400 Bad Request si el modelo de consulta no es válido.</item>
        /// <item>404 Not Found si no se encuentran productos.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] QueryObjectProduct query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var products = await _productRepository.GetProducts(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que permite agregar un nuevo producto a la base de datos.
        /// </summary>
        /// <param name="productDto">Parámetro de tipo productDto que sirve para representar el producto que se quiere agregar a la base de datos.</param>
        /// <returns>
        /// Retorna el producto que se agregó a la base de datos.
        /// <list type="bullet">
        /// <item>200 OK con el producto que se agregó a la base de datos.</item>
        /// <item>400 Bad Request si el modelo de producto no es válido o le falta su imagen.</item>
        /// <item>404 Not Found si no se encuentra el tipo de producto.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductRequestDto productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (productDto.Image == null || productDto.Image.Length == 0)
                {
                    return BadRequest( new { message = "La imagen es requerida"});
                }

                if (productDto.Image.ContentType != "image/jpeg" && productDto.Image.ContentType != "image/png")
                {
                    return BadRequest( new { message = "La imagen debe ser un archivo jpeg o png"});
                }

                if (productDto.Image.Length > 2 * 1024 * 1024)
                {
                    return BadRequest( new { message = "La imagen debe tener menos de 2 MB"});
                }

                // Crear un objeto de tipo ImageUploadParams que permita subir la imagen del producto a Cloudinary.
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(productDto.Image.FileName, productDto.Image.OpenReadStream()),
                    Folder = "E-commerce-Products"
                };

                // Subir la imagen del producto a Cloudinary.
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Verificar si ocurrió un error al subir la imagen.
                if (uploadResult.Error != null)
                {
                    return BadRequest( new { message = uploadResult.Error.Message});
                }

                var product = productDto.ToProductFromCreateDto();
                var ProductAdded = await _productRepository.AddProduct(product, uploadResult);
                return Ok(ProductAdded.ToProductDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            
        }

        /// <summary>
        /// Endpoint que permite actualizar un producto en la base de datos.
        /// </summary>
        /// <param name="id">Parámetro que representa la id del producto que se quiere actualizar</param>
        /// <param name="product">Parámetro que representa los datos que se quieren actualizar del producto.</param>
        /// <returns>
        /// Retorna el producto que se actualizó en la base de datos.
        /// <list type="bullet">
        /// <item>200 OK con el producto que se actualizó en la base de datos.</item>
        /// <item>400 Bad Request si el modelo de producto no es válido o le falta su imagen.</item>
        /// <item>404 Not Found si no se encuentra el producto o el tipo de producto.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </returns>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromForm] UpdateProductRequestDto product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (product.Image == null || product.Image.Length == 0)
                {
                    return BadRequest( new { message = "La imagen es requerida"});
                }

                if (product.Image.ContentType != "image/jpeg" && product.Image.ContentType != "image/png")
                {
                    return BadRequest( new { message = "La imagen debe ser un archivo jpeg o png"});
                }

                if (product.Image.Length > 2 * 1024 * 1024)
                {
                    return BadRequest( new { message = "La imagen debe tener menos de 2 MB"});
                }

                // Crear un objeto de tipo ImageUploadParams que permita subir la imagen del producto a Cloudinary.
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(product.Image.FileName, product.Image.OpenReadStream()),
                    Folder = "E-commerce-Products"
                };

                // Subir la imagen del producto a Cloudinary.
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                // Verificar si ocurrió un error al subir la imagen.
                if (uploadResult.Error != null)
                {
                    return BadRequest( new { message = uploadResult.Error.Message});
                }

                var existingProduct = await _productRepository.UpdateProduct(id, product, uploadResult);
                
                if (existingProduct == null)
                {
                    return NotFound( new { message = "No se encontró el producto"});
                }

                return Ok(existingProduct.ToProductDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint que permite eliminar un producto de la base de datos.
        /// </summary>
        /// <param name="id">Parámetro que representa la id del producto que se quiere eliminar.</param>
        /// <returns>
        /// Retorna el producto que se eliminó de la base de datos.
        /// <list type="bullet">
        /// <item>200 OK con el producto que se eliminó de la base de datos.</item>
        /// <item>404 Not Found si no se encuentra el producto.</item>
        /// <item>500 Internal Server Error si ocurre un error inesperado.</item>
        /// </returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var product = await _productRepository.DeleteProduct(id);

                if (product == null)
                {
                    return NotFound( new { message = "No se encontró el producto"});
                }
                
                return Ok(product.ToProductDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}