using LandRegistrySystem_Infrastructure.Context;
using LandRegistrySystem_Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public GenericRepository(AppDbContext db)
        {
            this._db = db;
            dbSet = _db.Set<T>();
        }
        public async Task CreateEntity(T entity)
        {

            await dbSet.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<T>> GetEntities(Expression<Func<T, bool>> filter = null, bool trakecd = true)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetEntity(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task RemoveEntity(T entity)
        {
            dbSet.Remove(entity);
            await _db.SaveChangesAsync();
        }
        public async Task RemoveEntities(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
            await _db.SaveChangesAsync();
        }

        public async Task SaveChanges()
        {
            await _db.SaveChangesAsync();
        }
    }
}
