namespace BBShop.Models;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid StoreId { get; set; }
    public Store Store { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
