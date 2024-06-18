using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace shopping2.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);
            _orderRepository = new OrderRepository(_context);
        }

        private void ClearDatabase()
        {
            _context.Orders.RemoveRange(_context.Orders);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_OrderExists_ReturnsOrder()
        {
            // Arrange
            ClearDatabase();
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId, Status = "Pending", UserId = "UserId", StoreId = Guid.NewGuid() };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal("Pending", result.Status);
        }

        [Fact]
        public async Task GetAllAsync_OrdersExist_ReturnsAllOrders()
        {
            // Arrange
            ClearDatabase();
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), Status = "Pending", UserId = "UserId", StoreId = Guid.NewGuid() },
                new Order { OrderId = Guid.NewGuid(), Status = "Completed", UserId = "UserId", StoreId = Guid.NewGuid() }
            };
            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.Status == "Pending");
            Assert.Contains(result, o => o.Status == "Completed");
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_AddsOrder()
        {
            // Arrange
            ClearDatabase();
            var order = new Order { OrderId = Guid.NewGuid(), Status = "Pending", UserId = "UserId", StoreId = Guid.NewGuid() };

            // Act
            await _orderRepository.AddAsync(order);
            var result = await _context.Orders.FindAsync(order.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Equal("Pending", result.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_OrderExists_UpdatesOrderStatus()
        {
            // Arrange
            ClearDatabase();
            var orderId = Guid.NewGuid();
            var order = new Order { OrderId = orderId, Status = "Pending", UserId = "UserId", StoreId = Guid.NewGuid() };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            order.Status = "Completed";
            await _orderRepository.UpdateStatusAsync(order);
            var result = await _context.Orders.FindAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal("Completed", result.Status);
        }

        [Fact]
        public async Task GetByStoreIdAsync_OrdersExistWithStoreId_ReturnsOrders()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Pending", UserId = "UserId" },
                new Order { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Completed", UserId = "UserId" }
            };
            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByStoreIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.Status == "Pending");
            Assert.Contains(result, o => o.Status == "Completed");
        }

        [Fact]
        public async Task GetByUserIdAsync_OrdersExistWithUserId_ReturnsOrders()
        {
            // Arrange
            ClearDatabase();
            var userId = "123";
            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), UserId = userId, Status = "Pending", StoreId = Guid.NewGuid() },
                new Order { OrderId = Guid.NewGuid(), UserId = userId, Status = "Completed", StoreId = Guid.NewGuid() }
            };
            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.Status == "Pending");
            Assert.Contains(result, o => o.Status == "Completed");
        }
    }
}