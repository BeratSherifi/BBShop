using Microsoft.AspNetCore.Http;

namespace BBShop.DTOs
{
    public class ProductUpdateDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile Image { get; set; } // Add this property for the product image
    }
}