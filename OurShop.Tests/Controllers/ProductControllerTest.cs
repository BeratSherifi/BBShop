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
    public class ProductControllerTest
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly ProductController _productController;

        public ProductControllerTest()
        {
            _productServiceMock = new Mock<IProductService>();
            _productController = new ProductController(_productServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ProductDto { ProductId = productId };
            _productServiceMock.Setup(service => service.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _productController.GetById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnValue.ProductId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productServiceMock.Setup(service => service.GetByIdAsync(productId)).ReturnsAsync((ProductDto)null);

            // Act
            var result = await _productController.GetById(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<ProductDto> { new ProductDto { ProductId = Guid.NewGuid() }, new ProductDto { ProductId = Guid.NewGuid() } };
            _productServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetByStoreName_ShouldReturnOkResult_WithListOfProducts()
        {
            // Arrange
            var storeName = "Test Store";
            var products = new List<ProductDto> { new ProductDto { StoreName = storeName } };
            _productServiceMock.Setup(service => service.GetByStoreNameAsync(storeName)).ReturnsAsync(products);

            // Act
            var result = await _productController.GetByStoreName(storeName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProductDto>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal(storeName, returnValue.First().StoreName);
        }

        [Fact]
        public async Task Add_ShouldReturnOkResult_WhenProductIsAdded()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto { ProductName = "Test Product", Description = "Description", Price = 100m, StockQuantity = 10 };
            var userId = "user1";

            _productServiceMock.Setup(service => service.AddAsync(productCreateDto, userId)).Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _productController.Add(productCreateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenProductIsUpdated()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductName = "Updated Product", Description = "Updated Description", Price = 120m, StockQuantity = 15 };
            var userId = "user1";

            _productServiceMock.Setup(service => service.UpdateAsync(productId, productUpdateDto, userId)).Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _productController.Update(productId, productUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenProductUpdateFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductName = "Updated Product", Description = "Updated Description", Price = 120m, StockQuantity = 15 };
            var userId = "user1";
            var errorMessage = "Update failed";

            _productServiceMock.Setup(service => service.UpdateAsync(productId, productUpdateDto, userId)).ThrowsAsync(new Exception(errorMessage));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _productController.Update(productId, productUpdateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenProductIsDeleted()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = "user1";

            _productServiceMock.Setup(service => service.DeleteAsync(productId, userId)).Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _productController.Delete(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenProductDeleteFails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = "user1";
            var errorMessage = "Delete failed";

            _productServiceMock.Setup(service => service.DeleteAsync(productId, userId)).ThrowsAsync(new Exception(errorMessage));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _productController.Delete(productId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errorMessage, badRequestResult.Value);
        }
    }
}
