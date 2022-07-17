using DataAccess.EF.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EF.Repositories;

public class ApplicationRepository<TEntityType> : IApplicationRepository<TEntityType> where TEntityType : class
{
    public ApplicationRepository(ApiContext db)
    {
        Context = db;
    }

    public ApiContext Context { get; }

    public async Task<TEntityType?> FindAsync(int id)
    {
        return await Context.FindAsync<TEntityType>(id);
    }

    public async Task AddAsync(TEntityType? entity)
    {
        if (entity != null) await Context.AddAsync(entity);
    }

    public void AddRange(IEnumerable<TEntityType> entities)
    {
        Context.AddRange(entities);
    }

    public void Remove(TEntityType? entity)
    {
        if (entity != null) Context.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntityType>? entities)
    {
        if (entities != null) Context.RemoveRange(entities);
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }

    public void SetModifiedState(TEntityType entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
    }
}