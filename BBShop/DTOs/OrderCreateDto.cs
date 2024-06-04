namespace BBShop.DTOs
{
    public class OrderCreateDto
    {
        public Guid StoreId { get; set; }
        public ICollection<OrderItemCreateDto> OrderItems { get; set; }
    }
}