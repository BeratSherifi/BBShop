namespace BBShop.Models;

public class Store
{
    public Guid StoreId { get; set; } = Guid.NewGuid();
    public string StoreName { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
