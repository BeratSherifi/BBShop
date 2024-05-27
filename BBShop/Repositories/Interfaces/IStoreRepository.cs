using BBShop.Models;

namespace BBShop.Repositories.Interfaces;

public interface IStoreRepository
{
    Task<Store> GetByIdAsync(Guid id);
    Task AddAsync(Store store);
    Task UpdateAsync(Store store);
    Task DeleteAsync(Store store);
    Task<IEnumerable<Store>> SearchByNameAsync(string name);
}