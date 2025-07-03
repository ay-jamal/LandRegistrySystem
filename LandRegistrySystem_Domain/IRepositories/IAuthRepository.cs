using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.IRepositories
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<LoginResponseDto> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
