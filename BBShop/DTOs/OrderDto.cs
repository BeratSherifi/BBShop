namespace BBShop.DTOs;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid StoreId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}