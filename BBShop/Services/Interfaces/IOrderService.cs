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
        Task<IEnumerable<OrderDto>> GetByStoreIdAsync(Guid storeId);
        Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId);
        Task<OrderDto> AddAsync(OrderCreateDto orderDto, string userId);
        Task UpdateStatusAsync(Guid id, string status);
    }
}