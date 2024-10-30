using api.src.DTOs.User;
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
        
        public MobileClientController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
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
                if (User.Identity?.IsAuthenticated == true) return BadRequest(new { message = "You have to logout." });

                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(deleteAccountDto.Email);
                if (user == null) return Unauthorized("Invalid email or password.");

                var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, deleteAccountDto.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed) return Unauthorized("Invalid email or password.");

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Account deleted successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Error deleting account.", errors = result.Errors });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the account.", error = ex.Message });
            }
        }
    }
}