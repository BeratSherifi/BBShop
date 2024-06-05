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
    public class StoreRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<DbSet<Store>> _mockSet;
        private readonly StoreRepository _storeRepository;

        public StoreRepositoryTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockSet = new Mock<DbSet<Store>>();
            _storeRepository = new StoreRepository(_mockContext.Object, null); // assuming mapper is not used in tests
        }

        private void SetupMockDbSet(IQueryable<Store> stores)
        {
            _mockSet.As<IQueryable<Store>>().Setup(m => m.Provider).Returns(stores.Provider);
            _mockSet.As<IQueryable<Store>>().Setup(m => m.Expression).Returns(stores.Expression);
            _mockSet.As<IQueryable<Store>>().Setup(m => m.ElementType).Returns(stores.ElementType);
            _mockSet.As<IQueryable<Store>>().Setup(m => m.GetEnumerator()).Returns(stores.GetEnumerator());
            _mockContext.Setup(c => c.Stores).Returns(_mockSet.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnStore_WhenStoreExists()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var stores = new List<Store>
            {
                new Store { StoreId = storeId, StoreName = "Test Store 1", UserId = "user1", CreatedAt = DateTime.Now }
            }.AsQueryable();

            SetupMockDbSet(stores);

            // Act
            var result = await _storeRepository.GetByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
        }

        [Fact]
        public async Task AddAsync_ShouldAddStore()
        {
            // Arrange
            var newStore = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 2", UserId = "user2", CreatedAt = DateTime.Now };

            // Act
            await _storeRepository.AddAsync(newStore);

            // Assert
            _mockSet.Verify(m => m.AddAsync(newStore, default), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStore()
        {
            // Arrange
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1", UserId = "user1", CreatedAt = DateTime.Now };

            // Act
            store.StoreName = "Updated Store";
            await _storeRepository.UpdateAsync(store);

            // Assert
            _mockSet.Verify(m => m.Update(store), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store 1", UserId = "user1", CreatedAt = DateTime.Now };
            var stores = new List<Store> { store }.AsQueryable();

            SetupMockDbSet(stores);

            // Act
            await _storeRepository.DeleteAsync(store);

            // Assert
            _mockSet.Verify(m => m.Remove(store), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task SearchByNameAsync_ShouldReturnStores_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var stores = new List<Store>
            {
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1", UserId = "user1", CreatedAt = DateTime.Now },
                new Store { StoreId = Guid.NewGuid(), StoreName = "Another Store", UserId = "user2", CreatedAt = DateTime.Now }
            }.AsQueryable();

            SetupMockDbSet(stores);

            // Act
            var result = await _storeRepository.SearchByNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, s => s.StoreName.Contains(storeName));
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnStore_WhenUserIdMatches()
        {
            // Arrange
            var userId = "user1";
            var stores = new List<Store>
            {
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1", UserId = userId, CreatedAt = DateTime.Now },
                new Store { StoreId = Guid.NewGuid(), StoreName = "Another Store", UserId = "user2", CreatedAt = DateTime.Now }
            }.AsQueryable();

            SetupMockDbSet(stores);

            // Act
            var result = await _storeRepository.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }
    }
}
