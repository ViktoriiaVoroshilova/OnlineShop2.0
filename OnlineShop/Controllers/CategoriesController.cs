using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(
            IApplicationRepository<Category> categoryRepository,
            IApplicationRepository<Item> itemRepository)
        {
            _categoryRepository = categoryRepository;
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _categoryRepository.Context.Categories.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryRepository.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _categoryRepository.SetModifiedState(category);

            try
            {
                await _categoryRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _categoryRepository.FindAsync(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent(); ;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var items = _itemRepository.Context.Items.Where(i => i.CategoryId.Equals(category.Id));
            _itemRepository.RemoveRange(items);
            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync();

            return NoContent();
        }

        private readonly IApplicationRepository<Category> _categoryRepository;
        private readonly IApplicationRepository<Item> _itemRepository;
    }
}
