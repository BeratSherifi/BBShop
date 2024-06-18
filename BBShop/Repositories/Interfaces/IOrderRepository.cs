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
        Task<IEnumerable<Order>> GetByStoreIdAsync(Guid storeId);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(Order order);
    }
}