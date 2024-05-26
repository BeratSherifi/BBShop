namespace BBShop.DTOs;

public class ProductCreateDto
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid StoreId { get; set; }
}
