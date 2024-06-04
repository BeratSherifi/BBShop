using System.ComponentModel.DataAnnotations;

namespace BBShop.Models
{
    public class Store
    {
        [Key]
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string LogoUrl { get; set; } // Add this line for logo URL
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}