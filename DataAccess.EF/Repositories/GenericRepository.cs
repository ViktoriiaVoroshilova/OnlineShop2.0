using System.Linq.Expressions;
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

    public virtual List<TEntityType> Get(
        Expression<Func<TEntityType, bool>>? filter = null,
        Func<IQueryable<TEntityType>, IOrderedQueryable<TEntityType>>? orderBy = null,
        string? includeProperties = "")
    {
        IQueryable<TEntityType> query = _dbSet;

        if (filter != null) query = query.Where(filter!);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            query = includeProperties!
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        }

        return orderBy != null ? orderBy(query!).ToList() : query.ToList()!;
    }

    public TEntityType? Find(int id)
    {
        return _dbSet.Find(id);
    }

    public void Add(TEntityType entity)
    {
        _context.Add(entity);
    }

    public void AddRange(IEnumerable<TEntityType> entities)
    {
        _context.AddRange(entities);
    }

    public void Remove(TEntityType entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntityType> entities)
    {
        _context.RemoveRange(entities);
    }

    public void Update(TEntityType entity)
    {
        
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    private readonly DbSet<TEntityType> _dbSet;
    private readonly ApiContext _context;
}