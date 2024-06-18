using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace shopping.Tests.Repositories
{
    public class StoreRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly StoreRepository _storeRepository;

        public StoreRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);

            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Store, Store>();
            });
            var mapper = mockMapper.CreateMapper();

            _storeRepository = new StoreRepository(_context, mapper);
        }

        private void ClearDatabase()
        {
            _context.Stores.RemoveRange(_context.Stores);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_StoreExists_ReturnsStore()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            // Act
            var result = await _storeRepository.GetByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
            Assert.Equal("Test Store", result.StoreName);
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_AddsStore()
        {
            // Arrange
            ClearDatabase();
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };

            // Act
            await _storeRepository.AddAsync(store);
            var result = await _context.Stores.FindAsync(store.StoreId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(store.StoreId, result.StoreId);
            Assert.Equal("Test Store", result.StoreName);
        }

        [Fact]
        public async Task UpdateAsync_StoreExists_UpdatesStore()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            // Act
            store.StoreName = "Updated Store";
            await _storeRepository.UpdateAsync(store);
            var result = await _context.Stores.FindAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
            Assert.Equal("Updated Store", result.StoreName);
        }

        [Fact]
        public async Task DeleteAsync_StoreExists_DeletesStore()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            // Act
            await _storeRepository.DeleteAsync(store);
            var result = await _context.Stores.FindAsync(storeId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SearchByNameAsync_StoresExistWithMatchingName_ReturnsStores()
        {
            // Arrange
            ClearDatabase();
            var store1 = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            var store2 = new Store { StoreId = Guid.NewGuid(), StoreName = "Another Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            _context.Stores.AddRange(store1, store2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _storeRepository.SearchByNameAsync("Test");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(result, s => s.StoreName == "Test Store");
        }

        [Fact]
        public async Task GetByUserIdAsync_StoresExistForUser_ReturnsStore()
        {
            // Arrange
            ClearDatabase();
            var userId = "UserId";
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store", UserId = userId, LogoUrl = "LogoUrl" };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            // Act
            var result = await _storeRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(store.StoreId, result.StoreId);
            Assert.Equal("Test Store", result.StoreName);
        }
    }
}
