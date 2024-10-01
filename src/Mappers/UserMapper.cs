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
                RoleName = user.Role.Name
            };   
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto createUserRequestDto)
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

        public static User ToUserFromUpdateDto(this UpdateUserRequestDto updateUserRequestDto)
        {
            return new User
            {
                Name = updateUserRequestDto.Name,
                Birth_Date = updateUserRequestDto.Birth_Date,
                Gender = updateUserRequestDto.Gender,
            };
        }
    }
}