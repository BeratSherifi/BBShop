using System.ComponentModel.DataAnnotations;

namespace BBShop.Models;

public class Store
{
    [Key]
    public Guid StoreId { get; set; }
    public string StoreName { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}