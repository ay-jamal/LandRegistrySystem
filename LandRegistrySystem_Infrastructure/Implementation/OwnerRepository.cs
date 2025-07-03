using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Context;
using LandRegistrySystem_Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class OwnerRepository : GenericRepository<Owner>, IOwnerRepository
    {
        private readonly AppDbContext _db;

        public OwnerRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PaginatedResult<OwnerDto>> GetOwners(PaginationRequest paginationRequest)
        {
            var query = _db.Owners.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationRequest.SearchValue))
            {
                var searchValue = paginationRequest.SearchValue.ToLower();
                query = query.Where(o =>
                    o.FullName.ToLower().Contains(searchValue) ||
                    o.NationalId.ToLower().Contains(searchValue) ||
                    o.PhoneNumber.ToLower().Contains(searchValue));
            }

            var projectedQuery = query.OrderBy(o => o.Id).Select(o => new OwnerDto
            {
                Id = o.Id,
                FullName = o.FullName,
                NationalId = o.NationalId,
                PhoneNumber = o.PhoneNumber,
                FarmsCount = o.Farms.Count,
                CreatedAt = o.CreatedAt,
            });

            if (paginationRequest.SkipPaged)
            {
                var allData = await projectedQuery.ToListAsync();
                return new PaginatedResult<OwnerDto>
                {
                    CurrentPage = 1,
                    PageSize = allData.Count,
                    TotalItems = allData.Count,
                    TotalPages = 1,
                    data = allData
                };
            }

            return await PaginationHelper.Paginate(
                projectedQuery,
                paginationRequest.Page,
                paginationRequest.PageSize
            );
        }
    }
}
