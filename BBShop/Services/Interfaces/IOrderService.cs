using BBShop.DTOs;
using BBShop.Models;

namespace BBShop.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task AddAsync(OrderCreateDto orderDto);
    Task UpdateAsync(Guid orderId, OrderDto orderDto);
    
    Task UpdateStatusAsync(Guid orderId, OrderStatusUpdateDto statusUpdateDto);  // New method
    Task DeleteAsync(Guid id);
}
