using BBShop.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<IEnumerable<OrderDto>> GetByStoreNameAsync(string storeName); // Update the method signature
        Task AddAsync(OrderCreateDto orderDto, string userId);
        Task UpdateStatusAsync(Guid orderId, string status);
        Task DeleteAsync(Guid id);
    }
}