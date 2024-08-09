using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CachingWebApi.Repository;

public interface IRepository<TEntity> where TEntity:class
{
    public Task<IEnumerable<TEntity>> GetAll();
    public Task<EntityEntry<TEntity>> Add(TEntity entity);
    public Task Remove(TEntity entity);
}