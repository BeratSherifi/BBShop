

namespace BBShop.DTOs
{
    public class OrderCreateDto
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public Guid StoreId { get; set; }
        public ICollection<OrderItemCreateDto> OrderItems { get; set; }
    }
}

