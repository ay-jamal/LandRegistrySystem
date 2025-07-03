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
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly AppDbContext _db;

        public CityRepository(AppDbContext db) : base(db)
        {
            _db = db;

        }
        public async Task<PaginatedResult<CityDto>> GetCitites(PaginationRequest paginationRequest)
        {
            var query = _db.Citites.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationRequest.SearchValue))
            {
                var searchValue = paginationRequest.SearchValue.ToLower();
                query = query.Where(f => f.Name.ToLower().Contains(searchValue) || f.CityNumber.ToLower().Contains(searchValue));
            }

            var projectedQuery = query
                .OrderBy(c => c.Id)
                .Select(c => new CityDto
                {
                    Id = c.Id,
                    CityNumber = c.CityNumber,
                    Name = c.Name,
                    ProjectsCount = c.Projects.Count(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    UpdatedByUser = c.UpdatedByUser,
                    
                });

            if (paginationRequest.SkipPaged)
            {
                var allData = await projectedQuery.ToListAsync();
                return new PaginatedResult<CityDto>
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
