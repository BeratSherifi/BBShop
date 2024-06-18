using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using BBShop.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace shopping2.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly IMapper _mapper;
        private readonly OrderService _orderService;
        private readonly AppDbContext _dbContext;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockStoreRepository = new Mock<IStoreRepository>();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new AppDbContext(options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDto>();
                cfg.CreateMap<OrderCreateDto, Order>();
                cfg.CreateMap<OrderItem, OrderItemDto>();
                cfg.CreateMap<OrderItemCreateDto, OrderItem>();
            });
            _mapper = mockMapper.CreateMapper();

            _orderService = new OrderService(_mockOrderRepository.Object, _mockStoreRepository.Object, _dbContext, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_OrderExists_ReturnsOrderDto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId, Status = "Pending", OrderItems = new List<OrderItem>() };
            var orderDto = _mapper.Map<OrderDto>(order);

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _orderService.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDto.OrderId, result.OrderId);
            Assert.Equal(orderDto.Status, result.Status);
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_ReturnsOrderDto()
        {
            // Arrange
            var orderCreateDto = new OrderCreateDto
            {
                StoreId = Guid.NewGuid(),
                OrderItems = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            };
            var userId = "123";
            var order = _mapper.Map<Order>(orderCreateDto);
            order.UserId = userId;
            order.OrderId = Guid.NewGuid(); // Generate the OrderId here
            order.Status = "Pending";
            var orderDto = _mapper.Map<OrderDto>(order);

            _mockOrderRepository.Setup(repo => repo.AddAsync(It.IsAny<Order>())).Callback<Order>(o => o.OrderId = order.OrderId).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.AddAsync(orderCreateDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDto.OrderId, result.OrderId);
            Assert.Equal(orderDto.Status, result.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_OrderExists_UpdatesOrderStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId, Status = "Pending" };
            var newStatus = "Completed";

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mockOrderRepository.Setup(repo => repo.UpdateStatusAsync(order)).Returns(Task.CompletedTask);

            // Act
            await _orderService.UpdateStatusAsync(orderId, newStatus);

            // Assert
            _mockOrderRepository.Verify(repo => repo.UpdateStatusAsync(It.Is<Order>(o => o.OrderId == orderId && o.Status == newStatus)), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_OrdersExist_ReturnsOrderDtos()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), Status = "Pending", OrderItems = new List<OrderItem>() },
                new Order { OrderId = Guid.NewGuid(), Status = "Completed", OrderItems = new List<OrderItem>() }
            };
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            _mockOrderRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDtos.Count(), result.Count());
        }

        [Fact]
        public async Task GetByStoreIdAsync_OrdersExist_ReturnsOrderDtos()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Pending", OrderItems = new List<OrderItem>() },
                new Order { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Completed", OrderItems = new List<OrderItem>() }
            };
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            _mockOrderRepository.Setup(repo => repo.GetByStoreIdAsync(storeId)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetByStoreIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDtos.Count(), result.Count());
        }

        [Fact]
        public async Task GetByUserIdAsync_OrdersExist_ReturnsOrderDtos()
        {
            // Arrange
            var userId = "123";
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), UserId = userId, Status = "Pending", OrderItems = new List<OrderItem>() },
                new Order { OrderId = Guid.NewGuid(), UserId = userId, Status = "Completed", OrderItems = new List<OrderItem>() }
            };
            var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

            _mockOrderRepository.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDtos.Count(), result.Count());
        }
    }
}