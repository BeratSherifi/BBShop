namespace BBShop.Models
{
    public class Order
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        // Foreign key for User
        public string UserId { get; set; } // Changed to string
        public User User { get; set; }

        // Foreign key for Store
        public Guid StoreId { get; set; }
        public Store Store { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }


}