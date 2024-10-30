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
                    return NotFound();
                }

                return Ok(saleItems);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Purchases not found.")
                {
                    return NotFound(new { message = ex.Message });
                }
                else if (ex.Message == "Sale Items not found.")
                {
                    return NotFound(new { message = ex.Message });
                }
                else if (ex.Message == "Products not found.")
                {
                    return NotFound(new { message = ex.Message });
                }
                else
                {
                    return StatusCode(500, new { message = "Internal Server Error." });
                }
            }
            
        }
    }
}