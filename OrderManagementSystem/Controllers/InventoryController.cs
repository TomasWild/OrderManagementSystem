using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.interfaces;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IMemoryCache _memoryCache;

    public InventoryController(IInventoryItemRepository inventoryItemRepository, IMemoryCache memoryCache)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _memoryCache = memoryCache;
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateInventoryItem([FromBody] InventoryItem inventoryItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var item = await _inventoryItemRepository.CreateAsync(inventoryItem);
        
        _memoryCache.Remove("inventory_all_items");

        return CreatedAtAction(nameof(GetInventoryItemById), new { id = item.Id }, item);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllInventoryItems()
    {
        const string cacheKey = "inventory_all_items";

        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<InventoryItem>? items))
        {
            return Ok(items);
        }

        items = await _inventoryItemRepository.GetAllAsync();

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
        _memoryCache.Set(cacheKey, items, cacheEntryOptions);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetInventoryItemById([FromRoute] int id)
    {
        var cacheKey = $"inventory_item_{id}";

        if (_memoryCache.TryGetValue(cacheKey, out InventoryItem? item))
        {
            return Ok(item);
        }
        
        item = await _inventoryItemRepository.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound($"Item with ID {id} not found");
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
        
        _memoryCache.Set(cacheKey, item, cacheEntryOptions);

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteInventoryItem([FromRoute] int id)
    {
        var item = await _inventoryItemRepository.DeleteAsync(id);

        if (item is null)
        {
            return NotFound($"Item with ID {id} not found");
        }
        
        _memoryCache.Remove("inventory_all_items");
        _memoryCache.Remove($"inventory_item_{id}");

        return NoContent();
    }
}