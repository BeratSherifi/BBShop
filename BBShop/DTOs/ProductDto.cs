namespace BBShop.DTOs
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string StoreName { get; set; } // Add this property for store name
        public string ImageUrl { get; set; } // Add this property for the product image
    }
}