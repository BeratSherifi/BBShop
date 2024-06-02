namespace BBShop.DTOs
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public Guid StoreId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }
}