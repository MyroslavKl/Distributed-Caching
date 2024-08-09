using CachingWebApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CachingWebApi.Repository;

public class Repository<T>:IRepository<T> where T : class
{
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<T>> GetAll()
    {
        var entities = await _context.Set<T>().ToListAsync();
        return entities;
    }

    public async Task<EntityEntry<T>> Add(T entity)
    {
        var value = await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return value;
    }

    public async Task Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}