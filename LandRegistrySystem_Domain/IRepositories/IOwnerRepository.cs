using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Domain.IRepositories
{
    public interface IOwnerRepository : IGenericRepository<Owner>
    {
        Task<PaginatedResult<OwnerDto>> GetOwners(PaginationRequest paginationRequest);
    }
}
