using BBShop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByStoreNameAsync(string storeName);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}