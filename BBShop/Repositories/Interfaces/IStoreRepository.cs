using BBShop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store> GetByIdAsync(Guid id);
        Task AddAsync(Store store);
        Task UpdateAsync(Store store);
        Task DeleteAsync(Store store);
        Task<IEnumerable<Store>> SearchByNameAsync(string query); // Add this method for searching stores by name
        Task<Store> GetByUserIdAsync(string userId);
    }
}