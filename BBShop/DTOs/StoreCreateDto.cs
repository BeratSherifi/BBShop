namespace BBShop.DTOs
{
    public class StoreCreateDto
    {
        public string StoreName { get; set; }
        public IFormFile Logo { get; set; }  // Add this property
    }
}