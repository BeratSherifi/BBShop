using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;

namespace BBShop.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
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

    public async Task AddAsync(OrderCreateDto orderDto)
    {
        var order = new Order
        {
            BuyerId = orderDto.BuyerId,
            Status = OrderStatus.Pending,  // Ensure the status is set to Pending
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in orderDto.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
            {
                throw new Exception($"Product with ID {item.ProductId} is not available or out of stock.");
            }

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

            order.OrderItems.Add(orderItem);
        }

        await _orderRepository.AddAsync(order);
    }
    public async Task UpdateAsync(Guid orderId, OrderDto orderDto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        order.Status = orderDto.Status;
        await _orderRepository.UpdateAsync(order);
    }
    
    public async Task UpdateStatusAsync(Guid orderId, OrderStatusUpdateDto statusUpdateDto)  // New method
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new Exception("Order not found");
        }

        order.Status = statusUpdateDto.Status;
        await _orderRepository.UpdateAsync(order);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _orderRepository.DeleteAsync(id);
    }
}
