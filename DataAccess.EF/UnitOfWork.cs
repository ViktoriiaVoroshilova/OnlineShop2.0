using DataAccess.EF.DataAccess;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;

namespace DataAccess.EF;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(
        ApiContext context,
        IGenericRepository<Category> categoryRepository,
        IGenericRepository<Item> itemRepository)
    {
        _context = context;
        CategoryRepository = categoryRepository;
        ItemRepository = itemRepository;
    }

    public IGenericRepository<Item> ItemRepository { get; }

    public IGenericRepository<Category> CategoryRepository { get; }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed;
    private readonly ApiContext _context;
}