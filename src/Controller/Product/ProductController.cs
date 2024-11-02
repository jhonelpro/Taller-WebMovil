using api.src.Helpers;
using api.src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller
{
    /// <summary>
    /// Controlador de productos que maneja los endpoints asociados a la visualización de productos 
    /// para los usuarios no autenticados.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo IProductRepository que permite la inyección de dependencias.
        /// </summary>
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Constructor de la clase ProductController que recibe un objeto de tipo IProductRepository.
        /// </summary>
        /// <param name="productRepository">Parámetro de tipo IProductRepository que servirá para inicializar el atributo _productRepository</param>
        public ProductController(IProductRepository productRepository)
        {
            // Inicializa el atributo _productRepository con el objeto de tipo IProductRepository recibido.
            _productRepository = productRepository;
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
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts([FromQuery] QueryObject query)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var products = await _productRepository.GetAvailableProducts(query);
                return Ok(products);
            }
            catch (Exception ex) 
            {
                if (ex.Message == "Product not found.")
                {
                    return NotFound(new { Message = ex.Message });
                }
                else if (ex.Message == "Product Type incorrect.")
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