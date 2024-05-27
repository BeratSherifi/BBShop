namespace BBShop.DTOs;

public class StoreDto
{
    public Guid StoreId { get; set; }
    public string StoreName { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}