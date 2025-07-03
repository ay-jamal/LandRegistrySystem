using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LandRegistrySystem_Infrastructure.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetEntities(Expression<Func<T, bool>> filter = null, bool trakecd = true);
        Task<T> GetEntity(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task CreateEntity(T entity);
        Task RemoveEntity(T entity);

        Task RemoveEntities(IEnumerable<T> entities);
        Task SaveChanges();
    }
}
