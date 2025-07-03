using LandRegistrySystem_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Helpers
{
    public class PaginationHelper
    {
        public static async Task<PaginatedResult<T>> Paginate<T>(IQueryable<T> query, int page, int pageSize)
        {
            int totalItems = await query.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            int skipCount = (page - 1) * pageSize;

            List<T> items = await query
                .Skip(skipCount)
                .Take(pageSize)
                .ToListAsync();

            PaginatedResult<T> result = new PaginatedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                data = items
            };

            return result;
        }
    }
}
