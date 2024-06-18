using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace shopping.Tests.Controllers
{
    public class StoreControllerTests
    {
        private readonly Mock<IStoreService> _mockStoreService;
        private readonly StoreController _storeController;

        public StoreControllerTests()
        {
            _mockStoreService = new Mock<IStoreService>();
            _storeController = new StoreController(_mockStoreService.Object);
        }

        private ClaimsPrincipal CreateClaimsPrincipal(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task GetById_StoreExists_ReturnsOkResult()
        {
            // Arrange
            var store = new StoreDto { StoreId = Guid.NewGuid(), StoreName = "Test Store" };
            _mockStoreService.Setup(s => s.GetByIdAsync(store.StoreId)).ReturnsAsync(store);

            // Act
            var result = await _storeController.GetById(store.StoreId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStore = Assert.IsType<StoreDto>(okResult.Value);
            Assert.Equal(store.StoreId, returnStore.StoreId);
        }

        [Fact]
        public async Task GetById_StoreDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockStoreService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StoreDto)null);

            // Act
            var result = await _storeController.GetById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidStore_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var storeCreateDto = new StoreCreateDto { StoreName = "Test Store" };
            var createdStore = new StoreDto { StoreId = Guid.NewGuid(), StoreName = "Test Store" };
            _mockStoreService.Setup(s => s.AddAsync(storeCreateDto, It.IsAny<string>())).ReturnsAsync(createdStore);

            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _storeController.Create(storeCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdStore.StoreId, ((StoreDto)createdAtActionResult.Value).StoreId);
        }

        [Fact]
        public async Task Update_StoreExistsAndAuthorized_ReturnsNoContentResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeUpdateDto = new StoreUpdateDto { StoreName = "Updated Store" };
            var storeDto = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "userId" };

            _mockStoreService.Setup(s => s.GetByIdAsync(storeId)).ReturnsAsync(storeDto);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _storeController.Update(storeId, storeUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_StoreDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockStoreService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StoreDto)null);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _storeController.Update(Guid.NewGuid(), new StoreUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_UserNotAuthorized_ReturnsForbidResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeUpdateDto = new StoreUpdateDto { StoreName = "Updated Store" };
            var storeDto = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "userId" };

            _mockStoreService.Setup(s => s.GetByIdAsync(storeId)).ReturnsAsync(storeDto);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("otherUserId", "Seller") }
            };

            // Act
            var result = await _storeController.Update(storeId, storeUpdateDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_StoreExistsAndAuthorized_ReturnsNoContentResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeDto = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "userId" };

            _mockStoreService.Setup(s => s.GetByIdAsync(storeId)).ReturnsAsync(storeDto);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _storeController.Delete(storeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_StoreDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockStoreService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StoreDto)null);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _storeController.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_UserNotAuthorized_ReturnsForbidResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeDto = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "userId" };

            _mockStoreService.Setup(s => s.GetByIdAsync(storeId)).ReturnsAsync(storeDto);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("otherUserId", "Seller") }
            };

            // Act
            var result = await _storeController.Delete(storeId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Search_StoresExistWithMatchingName_ReturnsOkResult()
        {
            // Arrange
            var stores = new List<StoreDto> { new StoreDto { StoreId = Guid.NewGuid(), StoreName = "Test Store" } };
            _mockStoreService.Setup(s => s.SearchStoresAsync("Test")).ReturnsAsync(stores);

            // Act
            var result = await _storeController.Search("Test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStores = Assert.IsAssignableFrom<IEnumerable<StoreDto>>(okResult.Value);
            Assert.Single(returnStores);
        }

        [Fact]
        public async Task Search_StoresDoNotExistWithMatchingName_ReturnsNotFoundResult()
        {
            // Arrange
            _mockStoreService.Setup(s => s.SearchStoresAsync("Test")).ReturnsAsync(new List<StoreDto>());

            // Act
            var result = await _storeController.Search("Test");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }   
    }
}
