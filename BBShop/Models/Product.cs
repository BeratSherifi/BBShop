namespace BBShop.Models
{
    public class Product
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}