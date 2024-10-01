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

            var addedUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            
            if (addedUser == null)
            {
                throw new Exception("Failed to add user");
            }

            return addedUser;
        }

        public async Task<User?> DisableUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }else if (user.IsActive == 0)
            {
                throw new Exception("User is already inactive");
            }

            user.IsActive = 0;
            await _context.SaveChangesAsync();

            var userDisabeld = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return userDisabeld;
        }

        public async Task<User?> EnableUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }else if (user.IsActive == 1)
            {
                throw new Exception("User is already active");
            }

            user.IsActive = 1;
            await _context.SaveChangesAsync();

            var userEnabled = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return userEnabled;
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefaultAsync(p => p.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
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

            var updatedUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return updatedUser;
        }
    }
}