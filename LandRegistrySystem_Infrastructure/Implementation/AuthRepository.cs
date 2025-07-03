using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class AuthRepository : IAuthRepository
    {
 
            private readonly AppDbContext _context;
            private readonly IConfiguration _configuration;

            public AuthRepository(AppDbContext context, IConfiguration configuration)
            {
                _context = context;
                _configuration = configuration;
            }

            public async Task<User> Register(User user, string password)
            {
                // Generate Salt & Hash Password
                using var hmac = new HMACSHA512();
                user.PasswordSalt = hmac.Key; // Store the salt
                user.Password = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }

            public async Task<LoginResponseDto> Login(string username, string password)
            {
                var user = await _context.Users.Include(u => u.Role)
                                               .FirstOrDefaultAsync(x => x.Username == username);

                if (user == null)
                    return null;

                // Check if user is active
                if (!user.IsActive)
                    return null; // Return null if the user is not active (unauthorized)

                // Validate Password
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                if (Convert.ToBase64String(computedHash) != user.Password)
                    return null;

                // Generate JWT Token
                string token = GenerateJwtToken(user);

                return new LoginResponseDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Adress = user.Adress,
                    RoleId = user.RoleId,
                    Token = token,
                };
            }
            private string GenerateJwtToken(User user)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Apisettings:secret"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
            {
              new Claim("userId", user.Id.ToString()),
    new Claim("username", user.Username),
    new Claim("role", user.RoleId.ToString()),
    new Claim("phone", user.Phone ?? ""),
    new Claim("email", user.Email ?? "")
            };

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            public async Task<bool> UserExists(string username)
            {
                return await _context.Users.AnyAsync(x => x.Username == username);
            }
        }
}
