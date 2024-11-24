using api.src.Helpers;
using api.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.Purchase
{
    /// <summary>
    /// Controlador para mostrar las compras realizadas por todos los usuarios del sistema.
    /// </summary>
    /// <remarks>
    /// Solo los usuarios con el rol de Admin pueden acceder a los endpoints de este controlador.
    /// El controlador cuenta con un único endpoint que permite mostrar todas las compras realizadas por los usuarios del sistema.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SaleDisplayController : ControllerBase
    {
        /// <summary>
        /// Atributo de tipo ISaleItem que permite la inyección de dependencias.
        /// </summary>
        private readonly ISaleItem _saleItem;

        /// <summary>
        /// Constructor de la clase SaleDisplayController que recibe un objeto de tipo ISaleItem.
        /// </summary>
        /// <param name="saleItem">Parámetro de tipo ISaleItem que sirve para inicializar el atributo _saleItem.</param>
        public SaleDisplayController(ISaleItem saleItem)
        {
            // Inicializa del atributo _saleItem.
            _saleItem = saleItem;
        }
        
        /// <summary>
        /// Endpoint para mostrar todas las compras realizadas por los usuarios del sistema.
        /// </summary>
        /// <returns>
        /// Retorna un objeto JSON con la información de las compras realizadas por los usuarios.
        /// <list type="Bullet">
        /// <item>200 Ok con la información de las compras realizadas.</item>
        /// <item>404 Not Found si no se encuentran compras realizadas.</item>
        /// <item>500 Internal Server Error si ocurre un error al intentar obtener la información de las compras realizadas.</item>
        /// </list>
        /// </returns>
        [HttpGet("SaleDisplay")]
        public async Task<IActionResult> SaleDisplay([FromQuery] QueryObjectSale queryObjectSale)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var purchases = await _saleItem.GetPurchasesAsyncForAdmin(queryObjectSale);

                if (purchases == null)
                {
                    return NotFound("No sales found.");
                }

                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}