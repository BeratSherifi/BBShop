namespace BBShop.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string BuyerId { get; set; }
    public User Buyer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
