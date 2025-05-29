using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.interfaces;

public interface IInventoryItemRepository
{
    Task<InventoryItem> CreateAsync(InventoryItem inventoryItem);

    Task<ICollection<InventoryItem>> GetAllAsync();
    
    Task<InventoryItem?> GetByIdAsync(int id);

    Task<InventoryItem?> DeleteAsync(int id);
}