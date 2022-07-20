using DataAccess.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        public CategoriesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _uow.CategoryRepository.GetAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _uow.CategoryRepository.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPut]
        public async Task<IActionResult> PutCategory(Category category)
        {
            try
            {
                _uow.CategoryRepository.Update(category);
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _uow.CategoryRepository.FindAsync(category.Id) == null) return NotFound();
                throw;
            }

            return NoContent(); ;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _uow.CategoryRepository.Add(category);
            await _uow.SaveAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _uow.CategoryRepository.FindAsync(id);
            if (category == null) return NotFound();

            var items = await _uow.ItemRepository.GetAsync(i => i.CategoryId.Equals(category.Id));
            _uow.ItemRepository.RemoveRange(items);
            _uow.CategoryRepository.Remove(category);
            await _uow.SaveAsync();

            return NoContent();
        }


        [HttpGet("{id:int}/Items")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems(int id, [FromQuery] int page, [FromQuery] int limit)
        {
            var items = await _uow.ItemRepository.GetAsync(i => i.CategoryId.Equals(id));
            return items
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
        }

        private readonly IUnitOfWork _uow;
    }
}
