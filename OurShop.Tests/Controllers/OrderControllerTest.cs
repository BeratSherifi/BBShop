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
    public class OrderControllerTest
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly OrderController _orderController;

        public OrderControllerTest()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _orderController = new OrderController(_orderServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WithOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new OrderDto { OrderId = orderId };
            _orderServiceMock.Setup(service => service.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _orderController.GetById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(orderId, returnValue.OrderId);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderServiceMock.Setup(service => service.GetByIdAsync(orderId)).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _orderController.GetById(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfOrders()
        {
            // Arrange
            var orders = new List<OrderDto> { new OrderDto { OrderId = Guid.NewGuid() }, new OrderDto { OrderId = Guid.NewGuid() } };
            _orderServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _orderController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetByStoreName_ShouldReturnOkResult_WithListOfOrders()
        {
            // Arrange
            var storeName = "Test Store";
            var orders = new List<OrderDto> { new OrderDto { StoreId = Guid.NewGuid() } };
            _orderServiceMock.Setup(service => service.GetByStoreNameAsync(storeName)).ReturnsAsync(orders);

            // Act
            var result = await _orderController.GetByStoreName(storeName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<OrderDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtActionResult_WithCreatedOrder()
        {
            // Arrange
            var orderCreateDto = new OrderCreateDto { StoreId = Guid.NewGuid(), OrderItems = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 } } };
            var orderDto = new OrderDto { OrderId = Guid.NewGuid(), UserId = "user1" };
            var userId = "user1";

            _orderServiceMock.Setup(service => service.AddAsync(orderCreateDto, userId)).ReturnsAsync(orderDto);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _orderController.Create(orderCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_orderController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(orderDto.OrderId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(orderDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnNoContent_WhenStatusIsUpdated()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var status = OrderStatus.Completed;

            _orderServiceMock.Setup(service => service.UpdateStatusAsync(orderId, status)).Returns(Task.CompletedTask);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Role, "Seller")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _orderController.UpdateStatus(orderId, status);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
