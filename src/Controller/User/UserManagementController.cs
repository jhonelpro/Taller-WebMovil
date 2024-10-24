using api.src.Helpers;
using api.src.Mappers;
using api.src.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UserManagementController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] QueryObjectUsers query)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _userManager.Users.AsQueryable();

                if (!string.IsNullOrEmpty(query.Name)) user = user.Where(p => p.Name!.Contains(query.Name));

                var usersList = await user.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

                var usersListDto = usersList.Select(u => UserMapper.MapUserToUserDto(u)).ToList();
                
                return Ok(usersListDto);                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ChangeStateUser/{email}")]
        public async Task<IActionResult> ChangeStateUser(string email)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(email)) return BadRequest("Email is required");

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                if (user.IsActive == 1){
                    user.IsActive = 0;
                } else {
                    user.IsActive = 1;
                }

                await _userManager.UpdateAsync(user);

                return Ok(UserMapper.MapUserToUserDto(user));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}