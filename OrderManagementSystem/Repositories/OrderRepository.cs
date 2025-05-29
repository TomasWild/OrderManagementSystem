using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.interfaces;

namespace OrderManagementSystem.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .ToListAsync();

        return orders;
    }

    public async Task<Order?> GetOrderByIdWithItemsAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }

    public async Task<Order?> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order is null)
        {
            return null;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return order;
    }
}