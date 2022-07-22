using DataAccess.EF;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Services;

public class CategoriesService : ICategoriesService
{
    public CategoriesService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task DeleteWithItemsAsync(int id)
    {
        var category = await _uow.CategoryRepository.FindAsync(id);
        if (category != null)
        {
            var items = await _uow.ItemRepository.Get().Where(i => i.CategoryId.Equals(category.Id)).ToListAsync();
            await _uow.ItemRepository.RemoveRangeAsync(items);
            await _uow.CategoryRepository.RemoveAsync(category);
            await _uow.SaveAsync();
        }
    }

    private readonly IUnitOfWork _uow;
}