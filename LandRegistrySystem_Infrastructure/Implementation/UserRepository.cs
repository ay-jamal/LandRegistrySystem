using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext db) : base(db)
        {
            _context = db;
        }

        // Get all users
        public async Task<List<UserDto>> GetAllUsersAsync(string? SearchValue)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(SearchValue))
            {
                query = query.Where(u => u.Username.Contains(SearchValue)
                    || u.Phone.Contains(SearchValue)
                    || u.Email.Contains(SearchValue)
                    || u.Username.Contains(SearchValue)
                    || u.FullName.Contains(SearchValue));
            }
            var users = await query
                .Include(u => u.Role) // Include Role to map RoleName
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Adress = user.Adress,
                    RoleId = user.RoleId,
                    RoleName = user.Role.Name,
                    IsActive = user.IsActive,
                })
                .ToListAsync(); // ✅ Await the execution

            return users; // ✅ Now correctly returns Task<List<UserDto>>

        }


        // Update user
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Delete user by ID
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

    }
}
