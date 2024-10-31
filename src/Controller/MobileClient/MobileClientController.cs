using System.Security.Claims;
using api.src.DTOs.User;
using api.src.Interfaces;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controller.MobileClient
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class MobileClientController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly ITicket _ticket;
        
        public MobileClientController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher, ITicket ticket)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
            _ticket = ticket;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return BadRequest(new { message = "No active session found." });
                }

                await _signInManager.SignOutAsync();
                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout.", error = ex.Message });
            }
        }
        

        [HttpDelete("delete account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto deleteAccountDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (User.Identity?.IsAuthenticated != true) return BadRequest(new { message = "You have to login." });

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return Unauthorized("User not found.");

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, deleteAccountDto.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed) return Unauthorized("Incorrect password.");

                if(deleteAccountDto.ConfirmDeleteAccount)
                {
                    var result = await _userManager.DeleteAsync(user);
                    await _signInManager.SignOutAsync();

                    if (result.Succeeded)
                    {
                        return Ok(new { message = "Account deleted successfully." });
                    }
                    else
                    {
                        return StatusCode(500, new { message = "Error deleting account.", errors = result.Errors });
                    }
                }
                return BadRequest(new { message = "Account deletion was not confirmed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the account.", error = ex.Message });
            }
        }

        [HttpGet("get tickets")]
        public async Task<IActionResult> GetTickets()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var tickets = await _ticket.GetTickets(user.Id);

            if (tickets == null)
            {
                return BadRequest("Tickets not found");
            }   

            return Ok(tickets);
        }
/**
        [HttpGet("view products")]
        public async Task<IActionResult> ViewTickets()
        {
            
        }
**/
    }
}