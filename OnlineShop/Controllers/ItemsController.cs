using DataAccess.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.EF.Models;
using OnlineShop.Services;

namespace OnlineShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        public ItemsController(IUnitOfWork uow, IItemsService itemsService)
        {
            _uow = uow;
            _itemsService = itemsService;
        }

        [HttpGet]
        public async Task<IEnumerable<Item>> GetItems()
        {
            return await _uow.ItemRepository.Get().ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Item?>> GetItem(int id)
        {
            return await _itemsService.GetAsync(id);    
        }

        [HttpPut]
        public async Task<IActionResult> PutItem(Item item)
        {
            try
            {
                await _uow.ItemRepository.UpdateAsync(item);
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
            await _uow.ItemRepository.AddAsync(item);
            await _uow.SaveAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemsService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("~/api/Categories/{categoryId:int}/Items")]
        public async Task<IEnumerable<Item>> GetItems(int categoryId, [FromQuery] int page, [FromQuery] int limit)
        {
            return await _itemsService.GetItemsOnPageAsync(categoryId, page, limit);
        }

        private readonly IUnitOfWork _uow;
        private readonly IItemsService _itemsService;
    }
}
