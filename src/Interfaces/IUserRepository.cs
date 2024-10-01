using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.DTOs;
using api.src.DTOs.User;
using api.src.Models;

namespace api.src.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetUsers();
        Task<User> AddUser(User user);
        Task<User?> UpdateUser(int id, UpdateUserRequestDto user);
        Task<User?> UpdatePassword(int id, UpdatePasswordRequestDto password);
        Task<User?> EnableUser(int id);
        Task<User?> DisableUser(int id);
        Task<User?> GetUserById(int id);
    }
}