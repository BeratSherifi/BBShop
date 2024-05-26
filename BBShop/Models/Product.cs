namespace BBShop.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public Store Store { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}