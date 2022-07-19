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
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            return _uow.CategoryRepository.Get().ToList();
        }

        [HttpGet("{id:int}")]
        public ActionResult<Category> GetCategory(int id)
        {
            var category = _uow.CategoryRepository.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpPut("{id:int}")]
        public IActionResult PutCategory(int id, Category? category)
        {
            if (category == null || id != category.Id) return BadRequest();

            try
            {
                _uow.CategoryRepository.Update(category);
                _uow.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_uow.CategoryRepository.Find(id) == null) return NotFound();
                throw;
            }

            return NoContent(); ;
        }

        [HttpPost]
        public ActionResult<Category> PostCategory(Category? category)
        {
            if (category == null) return BadRequest();

            _uow.CategoryRepository.Add(category);
            _uow.Save();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }


        [HttpDelete("{id:int}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _uow.CategoryRepository.Find(id);
            if (category == null) return NotFound();

            var items = _uow.ItemRepository.Get(i => i.CategoryId.Equals(category.Id));
            _uow.ItemRepository.RemoveRange(items);
            _uow.CategoryRepository.Remove(category);
            _uow.Save();

            return NoContent();
        }

        private readonly IUnitOfWork _uow;
    }
}
