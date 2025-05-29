namespace OrderManagementSystem.Models;

public class InventoryItem
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public int Quantity { get; set; }
    public required string Location { get; set; }

    public void DisplayInfo()
    {
        Console.WriteLine($"{Name} | Quantity: {Quantity} | Location: {Location}");
    }
}