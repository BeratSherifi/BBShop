using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace shopping2.Tests.Services
{
    public class StoreServiceTests
    {
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<IMapper> _mockMapper;
        private readonly StoreService _storeService;

        public StoreServiceTests()
        {
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockMapper = new Mock<IMapper>();
            _storeService = new StoreService(_mockStoreRepository.Object, _mockEnvironment.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_StoreExists_ReturnsStoreDto()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store" };
            var storeDto = new StoreDto { StoreId = storeId, StoreName = "Test Store" };

            _mockStoreRepository.Setup(repo => repo.GetByIdAsync(storeId)).ReturnsAsync(store);
            _mockMapper.Setup(m => m.Map<StoreDto>(store)).Returns(storeDto);

            // Act
            var result = await _storeService.GetByIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeDto.StoreId, result.StoreId);
            Assert.Equal(storeDto.StoreName, result.StoreName);
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_ReturnsStoreDto()
        {
            // Arrange
            var storeCreateDto = new StoreCreateDto
            {
                StoreName = "New Store"
            };
            var userId = "123";
            var store = new Store { StoreId = Guid.NewGuid(), StoreName = storeCreateDto.StoreName, UserId = userId };
            var storeDto = new StoreDto { StoreId = store.StoreId, StoreName = store.StoreName, UserId = userId };

            _mockMapper.Setup(m => m.Map<Store>(storeCreateDto)).Returns(store);
            _mockStoreRepository.Setup(repo => repo.AddAsync(store)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<StoreDto>(store)).Returns(storeDto);

            // Act
            var result = await _storeService.AddAsync(storeCreateDto, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeDto.StoreId, result.StoreId);
            Assert.Equal(storeDto.StoreName, result.StoreName);
            Assert.Equal(storeDto.UserId, result.UserId);
        }

        [Fact]
        public async Task DeleteAsync_StoreExists_DeletesStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store" };

            _mockStoreRepository.Setup(repo => repo.GetByIdAsync(storeId)).ReturnsAsync(store);
            _mockStoreRepository.Setup(repo => repo.DeleteAsync(store)).Returns(Task.CompletedTask);

            // Act
            await _storeService.DeleteAsync(storeId);

            // Assert
            _mockStoreRepository.Verify(repo => repo.DeleteAsync(store), Times.Once);
        }

        [Fact]
        public async Task SearchStoresAsync_StoresFound_ReturnsStoreDtos()
        {
            // Arrange
            var query = "Test";
            var stores = new List<Store>
            {
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1" },
                new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 2" }
            };
            var storeDtos = stores.Select(s => new StoreDto { StoreId = s.StoreId, StoreName = s.StoreName });

            _mockStoreRepository.Setup(repo => repo.SearchByNameAsync(query)).ReturnsAsync(stores);
            _mockMapper.Setup(m => m.Map<IEnumerable<StoreDto>>(stores)).Returns(storeDtos);

            // Act
            var result = await _storeService.SearchStoresAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(storeDtos.Count(), result.Count());
        }
    }
}