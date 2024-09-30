using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Mappers;
using api.src.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => u.ToUserDto())
                .ToListAsync();
                
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserRequestDto userRequestDto)
        {
            var user = userRequestDto.ToProductFromCreateDto();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}