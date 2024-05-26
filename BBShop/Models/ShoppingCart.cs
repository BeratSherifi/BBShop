namespace BBShop.Models;

public class ShoppingCart
{
    public Guid CartId { get; set; } //Primary Key
    public string BuyerId { get; set; }
    public User Buyer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}