using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Repositories
{
    public class OrderRepositoryTest
    {
        private readonly OrderRepository _orderRepository;
        private readonly AppDbContext _context;

        public OrderRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Order")
                .Options;

            _context = new AppDbContext(options);
            _orderRepository = new OrderRepository(_context);

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _context.Orders.RemoveRange(_context.Orders);
            _context.SaveChanges();

            var user = new User { Id = "user1", UserName = "user1@example.com", FullName = "User One", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store", UserId = user.Id, LogoUrl = "logo.png", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };
            _context.Users.Add(user);
            _context.Stores.Add(store);
            _context.SaveChanges();

            var orders = new List<Order>
            {
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = user.Id, StoreId = store.StoreId },
                new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Completed", UserId = user.Id, StoreId = store.StoreId }
            };

            _context.Orders.AddRange(orders);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            SeedDatabase();
            // Arrange
            var orderId = _context.Orders.First().OrderId;

            // Act
            var result = await _orderRepository.GetByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            SeedDatabase();
            // Act
            var result = await _orderRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            SeedDatabase();
            // Arrange
            var newOrder = new Order { OrderId = Guid.NewGuid(), OrderDate = DateTime.Now, Status = "Pending", UserId = "user1", StoreId = _context.Stores.First().StoreId };

            // Act
            await _orderRepository.AddAsync(newOrder);

            // Assert
            Assert.Equal(3, _context.Orders.Count());
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateOrderStatus()
        {
            SeedDatabase();
            // Arrange
            var order = _context.Orders.First();
            order.Status = "Shipped";

            // Act
            await _orderRepository.UpdateStatusAsync(order);

            // Assert
            var updatedOrder = _context.Orders.First();
            Assert.Equal("Shipped", updatedOrder.Status);
        }

       
    }
}
