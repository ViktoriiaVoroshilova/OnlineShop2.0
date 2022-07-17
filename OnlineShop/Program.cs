using System.Text.Json.Serialization;
using DataAccess.EF.DataAccess;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("OnlineShop"));
builder.Services.AddScoped<IApplicationRepository<Category>, ApplicationRepository<Category>>();
builder.Services.AddScoped<IApplicationRepository<Item>, ApplicationRepository<Item>>();

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

InitTestData(app);

app.Run();

void InitTestData(WebApplication app)
{
    var score = app.Services.CreateScope();
    var context = score.ServiceProvider.GetService<ApiContext>();
    var itemRepository = score.ServiceProvider.GetService<IApplicationRepository<Item>>();
    var categoryRepository = score.ServiceProvider.GetService<IApplicationRepository<Category>>();

    if (context == null)
    {
        return;
    }

    var categories = new List<Category>
    {
        new() { Id = 1, Name = "Books" },
        new() { Id = 2, Name = "Shoes" },
        new() { Id = 3, Name = "Sport" }
    };

    var items = new List<Item>
    {
        new() { Id = 1, Name = "Война и мир", Price = 1999.99, CategoryId = 1 },
        new() { Id = 2, Name = "Преступление и наказание", Price = 1599.99, CategoryId = 1 },
        new() { Id = 3, Name = "Колобок", Price = 60.0, CategoryId = 1 },
        new() { Id = 4, Name = "Математический Анализ", Price = 549.99, CategoryId = 1 },
        new() { Id = 5, Name = "Сказки народов мира", Price = 1949.99, CategoryId = 1 },
        new() { Id = 6, Name = "Сказки народов мираю Подарочное издание", Price = 5999.99, CategoryId = 1 },
        new() { Id = 7, Name = "Ecco", Price = 13000, CategoryId = 2 },
        new() { Id = 8, Name = "Officine Creative", Price = 49000, CategoryId = 2 },
        new() { Id = 9, Name = "Prada", Price = 190000, CategoryId = 2 }
    };
    
    categoryRepository!.AddRange(categories);
    itemRepository!.AddRange(items);
    context.SaveChanges();
}
