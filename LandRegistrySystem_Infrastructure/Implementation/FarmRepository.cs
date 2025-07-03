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
    public class FarmRepository : GenericRepository<Farm>, IFarmRepository
    {
        private readonly AppDbContext _db;

        public FarmRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PaginatedResult<FarmDto>> GetFarms(int? cityId, int? projectId, PaginationRequest paginationRequest)
        {
            var query = _db.Farms
                .Include(f => f.Owner)
                .Include(f => f.Boundaries)
                .Include(f => f.Project)
                    .ThenInclude(p => p.City)
                .AsNoTracking();

            // ✅ فلترة حسب المدينة إن وجدت
            if (cityId.HasValue)
            {
                query = query.Where(f => f.Project.CityId == cityId.Value);
            }

            // ✅ فلترة حسب المشروع إن وجد
            if (projectId.HasValue)
            {
                query = query.Where(f => f.ProjectId == projectId.Value);
            }

            // ✅ البحث النصي إن وجد
            if (!string.IsNullOrWhiteSpace(paginationRequest.SearchValue))
            {
                var searchValue = paginationRequest.SearchValue.ToLower();
                query = query.Where(f =>
                    f.FarmNumber.ToString().Contains(searchValue) ||
                    f.Owner.FullName.ToLower().Contains(searchValue));
            }

            var projectedQuery = query.OrderBy(f => f.Id).Select(f => new FarmDto
            {
                Id = f.Id,
                Area = f.Area,
                FarmNumber = f.FarmNumber,
                ProjectNumber = f.Project.ProjectNumber,
                ProjectId = f.Project.Id,
                CityNumber = f.Project.City.CityNumber,
                CreatedAt = f.CreatedAt,
                CityId = f.Project.City.Id,
                ProjectName = f.Project.Name,
                CityName = f.Project.City.Name,
                Boundaries = new BoundaryDto
                {
                    North = f.Boundaries.North,
                    South = f.Boundaries.South,
                    East = f.Boundaries.East,
                    West = f.Boundaries.West,
                },
                Owner = new OwnerDto
                {
                    Id = f.Owner.Id,
                    FullName = f.Owner.FullName,
                    NationalId = f.Owner.NationalId,
                    PhoneNumber = f.Owner.PhoneNumber
                }
            });

            if (paginationRequest.SkipPaged)
            {
                var allData = await projectedQuery.ToListAsync();
                return new PaginatedResult<FarmDto>
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
