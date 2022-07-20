using System.Linq.Expressions;

namespace DataAccess.EF.Repositories;

public interface IGenericRepository<TEntityType>
{
    public Task<IEnumerable<TEntityType>> GetAsync(
        Expression<Func<TEntityType, bool>>? filter = null,
        Func<IQueryable<TEntityType>, IOrderedQueryable<TEntityType>>? orderBy = null,
        string? includeProperties = "");

    public Task<TEntityType?> FindAsync(int id);

    public void Add(TEntityType entity);

    public void AddRange(IEnumerable<TEntityType> entities);

    public void Remove(TEntityType entity);

    public void RemoveRange(IEnumerable<TEntityType> entities);

    public void Update(TEntityType entity);
}