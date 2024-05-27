using BBShop.DTOs;

namespace BBShop.Services.Interfaces;

public interface IStoreService
{
    Task<StoreDto> GetByIdAsync(Guid id);
    Task<StoreDto> AddAsync(StoreCreateDto storeDto, string userId);
    Task UpdateAsync(Guid id, StoreUpdateDto storeDto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<StoreDto>> SearchByNameAsync(string name);
}
