using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs.User;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
    }
}