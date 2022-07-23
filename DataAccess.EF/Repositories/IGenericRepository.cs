namespace DataAccess.EF.Repositories;

public interface IGenericRepository<TEntityType>
{
    public IQueryable<TEntityType> Get();

    public Task<TEntityType?> FindAsync(int id);

    public Task AddAsync(TEntityType entity);

    public Task AddRangeAsync(IEnumerable<TEntityType> entities);

    public Task RemoveAsync(TEntityType? entity);

    public Task RemoveRangeAsync(IEnumerable<TEntityType> entities);

    public Task UpdateAsync(TEntityType entity);
}