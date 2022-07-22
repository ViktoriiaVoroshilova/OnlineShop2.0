using DataAccess.EF.Models;

namespace OnlineShop.Services;

public interface IItemsService
{
    Task<IEnumerable<Item>> GetItemsOnPageAsync(int categoryId, int page, int limit);

    Task DeleteAsync(int id);

    Task<Item?> GetAsync(int id);
}