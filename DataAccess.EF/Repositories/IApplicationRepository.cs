using DataAccess.EF.DataAccess;

namespace DataAccess.EF.Repositories;

public interface IApplicationRepository<TEntityType>
{
    public ApiContext Context { get; }

    public Task<TEntityType?> FindAsync(int id);

    public Task AddAsync(TEntityType? entity);

    public void AddRange(IEnumerable<TEntityType> entities);

    public void Remove(TEntityType? entity);

    public void RemoveRange(IEnumerable<TEntityType>? entities);

    public Task SaveChangesAsync();

    public void SetModifiedState(TEntityType entity);
}