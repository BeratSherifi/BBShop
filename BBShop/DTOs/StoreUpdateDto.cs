namespace BBShop.DTOs
{
    public class StoreUpdateDto
    {
        public string StoreName { get; set; }
        public IFormFile Logo { get; set; }  // Add this property
    }
}