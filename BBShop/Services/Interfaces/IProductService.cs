using BBShop.DTOs;

namespace BBShop.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetAllAsync();
    
    Task<IEnumerable<ProductDto>> GetByStoreIdAsync(Guid storeId);
    Task AddAsync(ProductCreateDto productDto);
    Task UpdateAsync(Guid productId, ProductUpdateDto productDto);
    Task DeleteAsync(Guid id);
}
