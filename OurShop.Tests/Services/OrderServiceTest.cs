using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Services
{
    public class OrderServiceTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IStoreRepository> _storeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _orderService;

        public OrderServiceTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _storeRepositoryMock = new Mock<IStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _storeRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId };
            var orderDto = new OrderDto { OrderId = orderId };
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            // Act
            var result = await _orderService.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order> { new Order(), new Order() };
            var orderDtos = new List<OrderDto> { new OrderDto(), new OrderDto() };
            _orderRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(orderDtos);

            // Act
            var result = await _orderService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ShouldReturnOrders_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var orders = new List<Order> { new Order { Store = new Store { StoreName = storeName } } };
            var orderDtos = new List<OrderDto> { new OrderDto { StoreId = orders.First().StoreId } };
            _orderRepositoryMock.Setup(repo => repo.GetByStoreNameAsync(storeName)).ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderDto>>(orders)).Returns(orderDtos);

            // Act
            var result = await _orderService.GetByStoreNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Equal(storeName, orders.First().Store.StoreName);
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            // Arrange
            var orderCreateDto = new OrderCreateDto
            {
                StoreId = Guid.NewGuid(),
                OrderItems = new List<OrderItemCreateDto>
                {
                    new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 }
                }
            };
            var order = new Order();
            var orderDto = new OrderDto();
            var userId = "user1";

            _mapperMock.Setup(m => m.Map<Order>(orderCreateDto)).Returns(order);
            _mapperMock.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);
            _orderRepositoryMock.Setup(repo => repo.AddAsync(order)).Returns(Task.CompletedTask);

            // Act
            var result = await _orderService.AddAsync(orderCreateDto, userId);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.AddAsync(order), Times.Once);
            Assert.Equal(orderDto, result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateOrderStatus()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId, Status = "Pending" };
            var status = "Shipped";

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            await _orderService.UpdateStatusAsync(orderId, status);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.UpdateStatusAsync(order), Times.Once);
            Assert.Equal(status, order.Status);
        }
    }
}
