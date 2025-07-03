using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<UserDto>> GetAllUsersAsync(string SearchValue);
        Task UpdateUserAsync(User user);
    }
}
