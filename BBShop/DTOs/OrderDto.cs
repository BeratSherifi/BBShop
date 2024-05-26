namespace BBShop.DTOs;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public string BuyerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; }
}
