using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Data;
using api.src.DTOs;
using api.src.Interfaces;
using api.src.Mappers;
using api.src.Models;
using Microsoft.EntityFrameworkCore;

namespace api.src.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<User> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<UserDto>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => u.ToUserDto())
                .ToListAsync();
        }

        public async Task<User?> UpdateUser(int id, UpdateUserRequestDto user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);
             
            if (existingUser == null) {
                throw new Exception("User not found");
            }

            existingUser.Name = user.Name;
            existingUser.Birth_Date = user.Birth_Date;
            existingUser.Gender = user.Gender;

            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}