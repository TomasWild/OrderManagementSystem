using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.interfaces;

namespace OrderManagementSystem.Repositories;

public class InventoryItemRepository : IInventoryItemRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem> CreateAsync(InventoryItem inventoryItem)
    {
        await _context.InventoryItems.AddAsync(inventoryItem);
        await _context.SaveChangesAsync();

        return inventoryItem;
    }

    public async Task<ICollection<InventoryItem>> GetAllAsync()
    {
        var items = await _context.InventoryItems
            .AsNoTracking()
            .ToListAsync();

        return items;
    }

    public async Task<InventoryItem?> GetByIdAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);

        return item;
    }

    public async Task<InventoryItem?> DeleteAsync(int id)
    {
        var item = await _context.InventoryItems.FindAsync(id);

        if (item is null)
        {
            return null;
        }

        _context.InventoryItems.Remove(item);
        await _context.SaveChangesAsync();

        return item;
    }
}