namespace BBShop.DTOs;

public class OrderCreateDto
{
    public string BuyerId { get; set; }
    public ICollection<OrderItemCreateDto> OrderItems { get; set; }
}
