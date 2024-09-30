using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Interfaces;
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
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            return Ok(users);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userRequestDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] CreateUserRequestDto userRequestDto)
        {
            var user = userRequestDto.ToUserFromCreateDto();
            await _userRepository.AddUser(user);
            return Ok(user.ToUserDto());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userRequestDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto userRequestDto)
        {
            var existingUser = await _userRepository.UpdateUser(id, userRequestDto);

            if (existingUser == null)
            {
                return NotFound();
            }
            return Ok(existingUser.ToUserDto());
        }
    }
}