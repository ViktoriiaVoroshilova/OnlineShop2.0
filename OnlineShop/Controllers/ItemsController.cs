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
        public async Task<IEnumerable<Item>> GetItems()
        {
            return await _uow.ItemRepository.GetAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Item?>> GetItem(int id)
        {
            var items = await _uow.ItemRepository.GetAsync(i => i.Id.Equals(id));
            return items.SingleOrDefault();
        }

        [HttpPut]
        public async Task<IActionResult> PutItem(Item item)
        {
            try
            {
                _uow.ItemRepository.Update(item);
                await _uow.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _uow.ItemRepository.FindAsync(item.Id) == null) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            _uow.ItemRepository.Add(item);
            await _uow.SaveAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _uow.ItemRepository.FindAsync(id);
            if (item == null) return NotFound();

            _uow.ItemRepository.Remove(item);
            await _uow.SaveAsync();

            return NoContent();
        }

        private readonly IUnitOfWork _uow;
    }
}
