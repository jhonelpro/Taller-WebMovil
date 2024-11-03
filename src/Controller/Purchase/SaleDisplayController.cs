using api.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.Purchase
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SaleDisplayController : ControllerBase
    {
        private readonly ISaleItem _saleItem;

        public SaleDisplayController(ISaleItem saleItem)
        {
            _saleItem = saleItem;
        }
        
        [HttpGet("SaleDisplay")]
        public async Task<IActionResult> SaleDisplay()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var saleItems = await _saleItem.GetPurchasesAsyncForAdmin();

                if (saleItems == null)
                {
                    return NotFound("No sales found.");
                }

                return Ok(saleItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}