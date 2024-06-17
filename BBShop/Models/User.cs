using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace BBShop.Models;

public class User : IdentityUser
{
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    [JsonIgnore]
    public ICollection<Store> Stores { get; set; } = new List<Store>();
    public ICollection<Product> Products { get; set; }
    
    public ICollection<Order> Orders { get; set; } 
}
