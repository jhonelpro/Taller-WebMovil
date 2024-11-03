using api.src.DTOs.User;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace api.src.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("edit-profile")]
        public async Task<IActionResult> EditProfile([FromBody] EditProfileDto editProfileDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (editProfileDto.DateOfBirth >= DateTime.Now) return BadRequest("Date of birth must be in the past.");

                if ((DateTime.Now.Year - editProfileDto.DateOfBirth.Year) < 13) return BadRequest("You must be at least 13 years old to register.");

                var user = await _userManager.GetUserAsync(User);

                if (user == null) return Unauthorized("User not found.");

                user.Name = editProfileDto.Name;
                user.DateOfBirth = editProfileDto.DateOfBirth;
                user.Gender = editProfileDto.Gender;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found.");

                var passwordVerification = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);

                if (!passwordVerification) return BadRequest("Current password is incorrect.");

                if (!string.Equals(changePasswordDto.NewPassword, changePasswordDto.ConfirmPassword, StringComparison.Ordinal)) return BadRequest("Passwords do not match");

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}