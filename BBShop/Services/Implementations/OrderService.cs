using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BBShop.Data;

namespace BBShop.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IStoreRepository storeRepository, AppDbContext context, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            var orderDto = _mapper.Map<OrderDto>(order);
            
            foreach (var item in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    item.ProductName = product.ProductName;
                }
            }

            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetByStoreIdAsync(Guid storeId)
        {
            var orders = await _orderRepository.GetByStoreIdAsync(storeId);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            foreach (var orderDto in orderDtos)
            {
                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.ProductName;
                    }
                }
            }

            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetByUserIdAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            foreach (var orderDto in orderDtos)
            {
                foreach (var item in orderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.ProductName;
                    }
                }
            }

            return orderDtos;
        }
        

        public async Task<OrderDto> AddAsync(OrderCreateDto orderDto, string userId)
        {
            var order = _mapper.Map<Order>(orderDto);
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";
            await _orderRepository.AddAsync(order);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task UpdateStatusAsync(Guid id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            order.Status = status;
            await _orderRepository.UpdateStatusAsync(order);
        }
    }
}
