namespace BBShop.DTOs
{
    public class StoreDto
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } // Add this line for UserName
        public string LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}