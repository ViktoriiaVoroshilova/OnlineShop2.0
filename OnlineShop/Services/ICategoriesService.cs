namespace OnlineShop.Services;

public interface ICategoriesService
{
    Task DeleteWithItemsAsync(int id);
}