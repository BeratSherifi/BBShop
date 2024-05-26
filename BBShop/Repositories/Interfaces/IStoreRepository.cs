using BBShop.Models;

namespace BBShop.Repositories.Interfaces;

// Repositories/Interfaces/IStoreRepository.cs
public interface IStoreRepository
{
    Task<Store> GetByIdAsync(Guid id);
    Task<IEnumerable<Store>> GetAllAsync();
    Task AddAsync(Store store);
    Task UpdateAsync(Store store);
    Task DeleteAsync(Guid id);
}
