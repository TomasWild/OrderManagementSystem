using System.Text;

namespace OrderManagementSystem.Models;

public class Order
{
    public int Id { get; init; }
    public required string CustomerName { get; set; }
    public DateTime DatePlaced { get; set; }
    public List<InventoryItem> Items { get; set; } = [];

    public void AddItem(InventoryItem item)
    {
        Items.Add(item);
    }

    public void RemoveItem(int itemId)
    {
        var itemToRemove = Items.FirstOrDefault(i => i.Id == itemId);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
        }
    }

    public string GetOrderSummary()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Order #{Id}");
        sb.AppendLine($"Customer: {CustomerName}");
        sb.AppendLine($"Date: {DatePlaced}");
        sb.AppendLine("Items:");

        foreach (var item in Items)
        {
            sb.AppendLine($" - {item.Name} (ID: {item.Id})");
        }

        sb.AppendLine("-------------------");

        return sb.ToString();
    }
}