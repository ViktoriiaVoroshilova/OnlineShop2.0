using System.Linq.Expressions;

namespace DataAccess.EF.Repositories;

public interface IGenericRepository<TEntityType>
{
    public List<TEntityType> Get(
        Expression<Func<TEntityType, bool>>? filter = null,
        Func<IQueryable<TEntityType>, IOrderedQueryable<TEntityType>>? orderBy = null,
        string? includeProperties = "");

    public TEntityType? Find(int id);

    public void Add(TEntityType entity);

    public void AddRange(IEnumerable<TEntityType> entities);

    public void Remove(TEntityType entity);

    public void RemoveRange(IEnumerable<TEntityType> entities);

    public void Update(TEntityType entity);
}