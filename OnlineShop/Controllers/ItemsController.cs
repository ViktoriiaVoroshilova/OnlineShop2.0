using DataAccess.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetItems([FromQuery] int categoryId, [FromQuery] int page, [FromQuery] int limit)
        {
            return _uow.ItemRepository
                .Get(i => i.CategoryId.Equals(categoryId))
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();
        }

        [HttpGet("{id:int}")]
        public ActionResult<Item?> GetItem(int id)
        {
            return _uow.ItemRepository.Get(i => i.Id.Equals(id)).SingleOrDefault();
        }

        [HttpPut("{id:int}")]
        public IActionResult PutItem(int id, Item? item)
        {
            if (item == null || id != item.Id) return BadRequest();

            try
            {
                _uow.ItemRepository.Update(item);
                _uow.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_uow.ItemRepository.Find(id) == null) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Item> PostItem(Item? item)
        {
            if (item == null) return BadRequest();

            _uow.ItemRepository.Add(item);
            _uow.Save();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteItem(int id)
        {
            var item = _uow.ItemRepository.Find(id);
            if (item == null) return NotFound();

            _uow.ItemRepository.Remove(item);
            _uow.Save();

            return NoContent();
        }

        private readonly IUnitOfWork _uow;
    }
}
