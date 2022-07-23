using DataAccess.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;
using OnlineShop.Filters;
using OnlineShop.Services;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(IUnitOfWork uow, ICategoriesService categoriesService)
        {
            _uow = uow;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _uow.CategoryRepository.Get().ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _uow.CategoryRepository.FindAsync(id);
            return category == null ? NotFound() : Ok(category);
        }

        [HttpPut]
        [ApiExceptionFilter]
        public async Task<IActionResult> PutCategory(Category category)
        {
            await _uow.CategoryRepository.UpdateAsync(category);
            await _uow.SaveAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            await _uow.CategoryRepository.AddAsync(category);
            await _uow.SaveAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoriesService.DeleteWithItemsAsync(id);
            return NoContent();
        }

        private readonly IUnitOfWork _uow;
        private readonly ICategoriesService _categoriesService;
    }
}
