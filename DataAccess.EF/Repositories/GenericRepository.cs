using DataAccess.EF.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EF.Repositories;

public class GenericRepository<TEntityType> : IGenericRepository<TEntityType> where TEntityType : class
{
    public GenericRepository(ApiContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntityType>();
    }

    public IQueryable<TEntityType> Get()
    {
        return _dbSet;
    }

    public async Task<TEntityType?> FindAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntityType entity)
    {
        await _context.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntityType> entities)
    {
        await _context.AddRangeAsync(entities);
    }

    public async Task RemoveAsync(TEntityType? entity)
    {
        if (entity == null) return;
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        await Task.FromResult(_dbSet.Remove(entity));
    }

    public async Task RemoveRangeAsync(IEnumerable<TEntityType> entities)
    {
        _dbSet.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(TEntityType entity)
    {
        
        _dbSet.Attach(entity);
        await Task.FromResult(_context.Entry(entity).State = EntityState.Modified);
    }

    private readonly DbSet<TEntityType> _dbSet;
    private readonly ApiContext _context;
}