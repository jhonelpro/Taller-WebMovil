using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs.User;
using api.src.Models.User;

namespace api.src.Mappers
{
    public class UserMapper
    {
        public static UserDto MapUserToUserDto(AppUser user)
        {
            return new UserDto
            {
                UserName = user.UserName ?? string.Empty,
                Rut = user.Rut,
                Name = user.Name,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Gender = user.Gender,
                IsActive = user.IsActive
            };
        }
    }
}