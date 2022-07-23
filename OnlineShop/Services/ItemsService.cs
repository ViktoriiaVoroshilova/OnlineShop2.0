using DataAccess.EF;
using DataAccess.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Services;

public class ItemsService : IItemsService
{
    public ItemsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Item>> GetItemsOnPageAsync(int categoryId, int page, int limit)
    {
        var previousItemsCount = page * limit - limit;

        return await _uow
            .ItemRepository
            .Get()
            .Where(i => i.CategoryId.Equals(categoryId))
            .Skip(previousItemsCount)
            .Take(limit)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _uow.ItemRepository.FindAsync(id);
        await _uow.ItemRepository.RemoveAsync(item);
        await _uow.SaveAsync();
    }

    public async Task<Item?> GetAsync(int id)
    {
        return await _uow
            .ItemRepository
            .Get()
            .SingleOrDefaultAsync(i => i.Id.Equals(id));
    }

    private readonly IUnitOfWork _uow;
}