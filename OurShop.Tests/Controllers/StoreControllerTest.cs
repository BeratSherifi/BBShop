using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace OurShop.Tests.Controllers
{
    public class StoreControllerTest
    {
        private readonly Mock<IStoreService> _storeServiceMock;
        private readonly StoreController _storeController;

        public StoreControllerTest()
        {
            _storeServiceMock = new Mock<IStoreService>();
            _storeController = new StoreController(_storeServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WithStore()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new StoreDto { StoreId = storeId };
            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync(store);

            // Act
            var result = await _storeController.GetById(storeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<StoreDto>(okResult.Value);
            Assert.Equal(storeId, returnValue.StoreId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenStoreDoesNotExist()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync((StoreDto)null);

            // Act
            var result = await _storeController.GetById(storeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtActionResult_WithCreatedStore()
        {
            // Arrange
            var storeCreateDto = new StoreCreateDto { StoreName = "Test Store" };
            var storeDto = new StoreDto { StoreId = Guid.NewGuid(), StoreName = storeCreateDto.StoreName, UserId = "user1" };
            var userId = "user1";

            _storeServiceMock.Setup(service => service.AddAsync(storeCreateDto, userId)).ReturnsAsync(storeDto);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _storeController.Create(storeCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_storeController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(storeDto.StoreId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(storeDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenStoreIsUpdated()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeUpdateDto = new StoreUpdateDto { StoreName = "Updated Store" };
            var store = new StoreDto { StoreId = storeId, StoreName = "Original Store", UserId = "user1" };

            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync(store);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _storeController.Update(storeId, storeUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var storeUpdateDto = new StoreUpdateDto { StoreName = "Updated Store" };
            var store = new StoreDto { StoreId = storeId, StoreName = "Original Store", UserId = "user1" };

            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync(store);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user2")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _storeController.Update(storeId, storeUpdateDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenStoreIsDeleted()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "user1" };

            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync(store);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _storeController.Delete(storeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var store = new StoreDto { StoreId = storeId, StoreName = "Test Store", UserId = "user1" };

            _storeServiceMock.Setup(service => service.GetByIdAsync(storeId)).ReturnsAsync(store);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user2")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _storeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _storeController.Delete(storeId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Search_ShouldReturnOkResult_WithListOfStores()
        {
            // Arrange
            var storeName = "Test Store";
            var stores = new List<StoreDto> { new StoreDto { StoreName = storeName } };
            _storeServiceMock.Setup(service => service.SearchByNameAsync(storeName)).ReturnsAsync(stores);

            // Act
            var result = await _storeController.Search(storeName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<StoreDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(storeName, returnValue.First().StoreName);
        }
    }
}
