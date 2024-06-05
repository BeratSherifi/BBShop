using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Repositories
{
    public class OrderRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<DbSet<Order>> _mockSet;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockSet = new Mock<DbSet<Order>>();
            _orderRepository = new OrderRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<Order> orders)
        {
            _mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            _mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            _mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            _mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());
            _mockContext.Setup(c => c.Orders).Returns(_mockSet.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order { OrderId = orderId, OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = Guid.NewGuid() }
            }.AsQueryable();

            SetupMockDbSet(orders);

            // Act
            var result = await _orderRepository.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = Guid.NewGuid() },
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Completed", UserId = "user2", StoreId = Guid.NewGuid() }
            }.AsQueryable();

            SetupMockDbSet(orders);

            // Act
            var result = await _orderRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ShouldReturnOrders_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = Guid.NewGuid(), Store = new Store { StoreName = storeName } },
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Completed", UserId = "user2", StoreId = Guid.NewGuid(), Store = new Store { StoreName = "Another Store" } }
            }.AsQueryable();

            SetupMockDbSet(orders);

            // Act
            var result = await _orderRepository.GetByStoreNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Equal(storeName, result.First().Store.StoreName);
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            // Arrange
            var newOrder = new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = Guid.NewGuid() };
            var orders = new List<Order>().AsQueryable();

            SetupMockDbSet(orders);

            // Act
            await _orderRepository.AddAsync(newOrder);

            // Assert
            _mockSet.Verify(m => m.AddAsync(newOrder, default), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateOrderStatus()
        {
            // Arrange
            var order = new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = Guid.NewGuid() };
            var orders = new List<Order> { order }.AsQueryable();

            SetupMockDbSet(orders);

            // Act
            order.Status = "Completed";
            await _orderRepository.UpdateStatusAsync(order);

            // Assert
            _mockSet.Verify(m => m.Update(order), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}
