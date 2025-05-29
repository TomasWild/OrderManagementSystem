using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.interfaces;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMemoryCache _memoryCache;

    public OrderController(IOrderRepository orderRepository, IMemoryCache memoryCache)
    {
        _orderRepository = orderRepository;
        _memoryCache = memoryCache;
    }

    [HttpPost]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (order.DatePlaced == default)
        {
            order.DatePlaced = DateTime.UtcNow;
        }

        var createdOrder = await _orderRepository.CreateOrderAsync(order);

        _memoryCache.Remove("orders_all");

        return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllOrders()
    {
        const string cacheKey = "orders_all";

        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Order>? orders))
        {
            return Ok(orders);
        }

        orders = await _orderRepository.GetAllOrdersAsync();

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
        _memoryCache.Set(cacheKey, orders, cacheEntryOptions);

        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var cacheKey = $"order_{id}";

        if (_memoryCache.TryGetValue(cacheKey, out Order? order))
        {
            return Ok(order);
        }

        order = await _orderRepository.GetOrderByIdWithItemsAsync(id);
        if (order is null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
        _memoryCache.Set(cacheKey, order, cacheEntryOptions);

        return Ok(order);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteOrder([FromRoute] int id)
    {
        var order = await _orderRepository.DeleteOrderAsync(id);

        if (order is null)
        {
            return NotFound($"Order with ID {id} not found");
        }

        _memoryCache.Remove("orders_all");
        _memoryCache.Remove($"order_{id}");

        return NoContent();
    }
}