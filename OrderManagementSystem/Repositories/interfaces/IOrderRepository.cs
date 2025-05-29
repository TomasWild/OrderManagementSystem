using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories.interfaces;

public interface IOrderRepository
{
    Task<Order> CreateOrderAsync(Order order);
    
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    
    Task<Order?> GetOrderByIdWithItemsAsync(int id);
    
    Task<Order?> DeleteOrderAsync(int id);
}