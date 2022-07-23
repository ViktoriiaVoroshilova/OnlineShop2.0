using DataAccess.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EF.DataAccess;


public class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Item> Items { get; set; } = null!;
}