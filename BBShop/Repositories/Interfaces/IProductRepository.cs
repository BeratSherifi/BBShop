using BBShop.Models;

namespace BBShop.Repositories.Interfaces;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    
    Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
