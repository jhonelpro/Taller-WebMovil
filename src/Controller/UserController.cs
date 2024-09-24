using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
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
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToArrayAsync();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        [HttpPost]
        public IActionResult CustomerRegistration([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email || u.Rut == user.Rut))
            {
                return BadRequest(new { message = "El correo electrónico o RUT ya está registrado." });
            }
             
            user.RoleId = 1;

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}