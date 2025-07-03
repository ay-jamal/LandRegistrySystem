using LandRegistrySystem_Domain.Dtos;
using LandRegistrySystem_Domain.Entities;
using LandRegistrySystem_Domain.IRepositories;
using LandRegistrySystem_Domain.Requests;
using LandRegistrySystem_Infrastructure.Context;
using LandRegistrySystem_Infrastructure.Helpers;
using LandRegistrySystem_Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class ProjectRepository: GenericRepository<Project>, IProjectRepository
    {
        private readonly AppDbContext _db;
        public ProjectRepository(AppDbContext db) : base(db)
        {
            _db = db;

        }
        public async Task<PaginatedResult<ProjectDto>> GetProjects(int? CityId,PaginationRequest paginationRequest)
        {
            IQueryable<Project> query = _db.Projects
                .Include(p => p.City)
                .Include(p => p.Farms)
                    .ThenInclude(f => f.Owner)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paginationRequest.SearchValue))
            {
                var search = paginationRequest.SearchValue.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Id.ToString().Contains(search));
            }

            if (CityId.HasValue)
            {
                query = query.Where(p =>p.CityId == CityId.Value);
            }

            var projectedQuery = query.OrderBy(p => p.Id).Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                ProjectNumber = p.ProjectNumber,
                FarmsCount = p.Farms.Count(),
                CreatedAt = p.CreatedAt,
                OwnersCount = p.Farms
                    .Select(f => f.OwnerId)
                    .Distinct()
                    .Count(),
                CityId = p.CityId,
                CityName = p.City.Name
            });

            if (paginationRequest.SkipPaged)
            {
                var allData = await projectedQuery.ToListAsync();
                return new PaginatedResult<ProjectDto>
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

        public async Task<Project?> GetByIdWithCityAsync(int projectId)
        {
            return await _db.Projects
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.Id == projectId);
        }

    }
}
