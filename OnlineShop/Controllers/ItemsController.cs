using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;
using DataAccess.EF.Repositories;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(IApplicationRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] int categoryId, [FromQuery] int page, [FromQuery] int limit)
        {
            return await _itemRepository
                .Context
                .Items
                .Where(i => i.CategoryId.Equals(categoryId))
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Item?>> GetItem(int id)
        {
            return await _itemRepository.Context.Items.SingleOrDefaultAsync(i => i.Id.Equals(id));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _itemRepository.SetModifiedState(item);

            try
            {
                await _itemRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _itemRepository.FindAsync(id) == null)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _itemRepository.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _itemRepository.Remove(item);
            await _itemRepository.SaveChangesAsync();

            return NoContent();
        }

        private readonly IApplicationRepository<Item> _itemRepository;
    }
}
