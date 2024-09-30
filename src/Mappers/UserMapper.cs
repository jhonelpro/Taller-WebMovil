using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.Models;

namespace api.src.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                Rut = user.Rut,
                Name = user.Name,
                Birth_Date = user.Birth_Date,
                Email = user.Email,
                Gender = user.Gender,
                Role = user.Role
            };   
        }

        public static User ToProductFromCreateDto(this CreateUserRequestDto createUserRequestDto)
        {
            return new User
            {
                Rut = createUserRequestDto.Rut,
                Name = createUserRequestDto.Name,
                Email = createUserRequestDto.Email,
                RoleId = createUserRequestDto.RoleId,
                Password = createUserRequestDto.Password
            };
        }
    }
}