using BBShop.DTOs;

namespace BBShop.Services.Interfaces;

// Services/Interfaces/IStoreService.cs
public interface IStoreService
{
    Task<StoreDto> GetByIdAsync(Guid id);
    Task<IEnumerable<StoreDto>> GetAllAsync();
    Task AddAsync(StoreCreateDto storeDto);
    Task UpdateAsync(StoreDto storeDto);
    Task DeleteAsync(Guid id);
}
