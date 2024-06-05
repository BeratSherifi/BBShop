using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Services
{
    public class StoreServiceTest
    {
        private readonly Mock<IStoreRepository> _storeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StoreService _storeService;

        public StoreServiceTest()
        {
            _storeRepositoryMock = new Mock<IStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            var environmentMock = new Mock<IWebHostEnvironment>();
            _storeService = new StoreService(_storeRepositoryMock.Object, environmentMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnStore_WhenStoreExists()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId };
            var storeDto = new StoreDto { StoreId = storeId };
            _storeRepositoryMock.Setup(repo => repo.GetByIdAsync(storeId)).ReturnsAsync(store);
            _mapperMock.Setup(m => m.Map<StoreDto>(store)).Returns(storeDto);

            // Act
            var result = await _storeService.GetByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeId, result.StoreId);
        }

        [Fact]
        public async Task AddAsync_ShouldAddStore()
        {
            // Arrange
            var storeCreateDto = new StoreCreateDto
            {
                StoreName = "Test Store"
            };
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = storeCreateDto.StoreName, UserId = "user1" };
            var storeDto = new StoreDto { StoreId = store.StoreId };
            var userId = "user1";

            _mapperMock.Setup(m => m.Map<Store>(storeCreateDto)).Returns(store);
            _mapperMock.Setup(m => m.Map<StoreDto>(store)).Returns(storeDto);
            _storeRepositoryMock.Setup(repo => repo.AddAsync(store)).Returns(Task.CompletedTask);

            // Act
            var result = await _storeService.AddAsync(storeCreateDto, userId);

            // Assert
            _storeRepositoryMock.Verify(repo => repo.AddAsync(store), Times.Once);
            Assert.Equal(storeDto, result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeUpdateDto = new StoreUpdateDto
            {
                StoreName = "Updated Store"
            };
            var store = new Store { StoreId = storeId, StoreName = "Original Store", UserId = "user1" };

            _storeRepositoryMock.Setup(repo => repo.GetByIdAsync(storeId)).ReturnsAsync(store);
            _mapperMock.Setup(m => m.Map(storeUpdateDto, store)).Callback<StoreUpdateDto, Store>((dto, s) =>
            {
                s.StoreName = dto.StoreName;
            });

            // Act
            await _storeService.UpdateAsync(storeId, storeUpdateDto);

            // Assert
            _storeRepositoryMock.Verify(repo => repo.UpdateAsync(store), Times.Once);
            Assert.Equal("Updated Store", store.StoreName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store", UserId = "user1" };

            _storeRepositoryMock.Setup(repo => repo.GetByIdAsync(storeId)).ReturnsAsync(store);
            _storeRepositoryMock.Setup(repo => repo.DeleteAsync(store)).Returns(Task.CompletedTask);

            // Act
            await _storeService.DeleteAsync(storeId);

            // Assert
            _storeRepositoryMock.Verify(repo => repo.DeleteAsync(store), Times.Once);
        }

        [Fact]
        public async Task SearchByNameAsync_ShouldReturnStores_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var stores = new List<Store> { new Store { StoreName = storeName } };
            var storeDtos = new List<StoreDto> { new StoreDto { StoreName = storeName } };
            _storeRepositoryMock.Setup(repo => repo.SearchByNameAsync(storeName)).ReturnsAsync(stores);
            _mapperMock.Setup(m => m.Map<IEnumerable<StoreDto>>(stores)).Returns(storeDtos);

            // Act
            var result = await _storeService.SearchByNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Equal(storeName, result.First().StoreName);
        }
    }
}
