using api.src.DTOs.Auth;
using api.src.DTOs.User;
using api.src.Interfaces;
using api.src.Models.User;
using Api.src.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.src.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (await _userManager.Users.AnyAsync(p => p.Email == registerDto.Email)) return BadRequest("Email already exists");

                if (string.IsNullOrEmpty(registerDto.Rut)) return BadRequest("RUT is required.");

                if (await _userManager.Users.AnyAsync(p => p.Rut == registerDto.Rut)) return BadRequest("Rut already exists");

                if (!RutValidations.IsValidRut(registerDto.Rut)) return BadRequest("Invalid Rut format or verification digit");

                if (registerDto.DateOfBirth >= DateTime.Now) return BadRequest("Date of birth must be in the past");

                if (string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.ConfirmPassword)) return BadRequest("Password is required");

                if (!string.Equals(registerDto.Password, registerDto.ConfirmPassword, StringComparison.Ordinal)) return BadRequest("Passwords do not match");

                var user = new AppUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    Rut = registerDto.Rut,
                    Name = registerDto.Name,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    IsActive = 1
                };

                var createUser = await _userManager.CreateAsync(user, registerDto.Password);

                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            UserName = user.UserName!,
                            Email = user.Email!,
                            Token = _tokenService.CreateToken(user)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }else
                {
                    return StatusCode(500, createUser.Errors);
                }

            } 
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try {

                if(!ModelState.IsValid) return BadRequest(ModelState);

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
                if(user == null) return Unauthorized("Invalid username or password.");


                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if(!result.Succeeded) return Unauthorized("Invalid username or password.");

                if(user.IsActive == 0) return Unauthorized("User is not active.");

                var token = _tokenService.CreateToken(user);

                if (string.IsNullOrEmpty(token)) return Unauthorized("Invalid token.");

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName!,
                        Email = user.Email!,
                        Token = token
                    }
                );
                
            }catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}