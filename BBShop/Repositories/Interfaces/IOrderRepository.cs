using BBShop.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByStoreNameAsync(string storeName); // Add this method
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}