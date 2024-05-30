using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IStoreRepository storeRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetByStoreNameAsync(string storeName)
        {
            var orders = await _orderRepository.GetByStoreNameAsync(storeName);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task AddAsync(OrderCreateDto orderDto, string userId)
        {
            var order = _mapper.Map<Order>(orderDto);
            order.UserId = userId; // Assign UserId directly
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

            await _orderRepository.AddAsync(order);
        }

        public async Task UpdateStatusAsync(Guid orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }
            order.Status = status;
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _orderRepository.DeleteAsync(id);
        }
    }
}
