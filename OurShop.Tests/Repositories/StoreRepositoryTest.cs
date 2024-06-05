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
    public class StoreRepositoryTest
    {
        private readonly StoreRepository _storeRepository;
        private readonly AppDbContext _context;

        public StoreRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Store")
                .Options;

            _context = new AppDbContext(options);
            _storeRepository = new StoreRepository(_context, null); // Assuming mapper is not needed for tests

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _context.Stores.RemoveRange(_context.Stores);
            _context.SaveChanges();

            var stores = new List<Store>
            {
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1", UserId = "user1", LogoUrl = "logo1.png", CreatedAt = DateTime.Now },
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 2", UserId = "user2", LogoUrl = "logo2.png", CreatedAt = DateTime.Now }
            };

            _context.Stores.AddRange(stores);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnStore_WhenStoreExists()
        {
            SeedDatabase();
            // Arrange
            var storeId = _context.Stores.First().StoreId;

            // Act
            var result = await _storeRepository.GetByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
        }

        [Fact]
        public async Task AddAsync_ShouldAddStore()
        {
            SeedDatabase();
            // Arrange
            var newStore = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 3", UserId = "user3", LogoUrl = "logo3.png", CreatedAt = DateTime.Now };

            // Act
            await _storeRepository.AddAsync(newStore);

            // Assert
            Assert.Equal(3, _context.Stores.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStore()
        {
            SeedDatabase();
            // Arrange
            var store = _context.Stores.First();
            store.StoreName = "Updated Store";

            // Act
            await _storeRepository.UpdateAsync(store);

            // Assert
            var updatedStore = _context.Stores.First();
            Assert.Equal("Updated Store", updatedStore.StoreName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStore()
        {
            SeedDatabase();
            // Arrange
            var storeId = _context.Stores.First().StoreId;
            var store = await _storeRepository.GetByIdAsync(storeId);

            // Act
            if (store != null)
            {
                await _storeRepository.DeleteAsync(store);
            }

            // Assert
            Assert.Equal(1, _context.Stores.Count());
        }

        [Fact]
        public async Task SearchByNameAsync_ShouldReturnStores_WhenStoreNameMatches()
        {
            SeedDatabase();
            // Arrange
            var storeName = "Test Store 1";

            // Act
            var result = await _storeRepository.SearchByNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, s => s.StoreName == storeName);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnStore_WhenUserIdMatches()
        {
            SeedDatabase();
            // Arrange
            var userId = "user1";

            // Act
            var result = await _storeRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }
    }
}
