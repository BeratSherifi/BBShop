using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace shopping2.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly OrderController _orderController;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _orderController = new OrderController(_mockOrderService.Object);
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
        public async Task GetById_OrderExists_ReturnsOkResult()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderDto = new OrderDto { OrderId = orderId, Status = "Pending" };
            _mockOrderService.Setup(s => s.GetByIdAsync(orderId)).ReturnsAsync(orderDto);

            // Act
            var result = await _orderController.GetById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrder = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(orderId, returnOrder.OrderId);
        }

        [Fact]
        public async Task GetById_OrderDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockOrderService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _orderController.GetById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_OrdersExist_ReturnsOkResult()
        {
            // Arrange
            var orders = new List<OrderDto>
            {
                new OrderDto { OrderId = Guid.NewGuid(), Status = "Pending" },
                new OrderDto { OrderId = Guid.NewGuid(), Status = "Completed" }
            };
            _mockOrderService.Setup(s => s.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await _orderController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnOrders.Count());
        }

        [Fact]
        public async Task GetByStoreId_OrdersExistWithStoreId_ReturnsOkResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var orders = new List<OrderDto>
            {
                new OrderDto { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Pending" },
                new OrderDto { OrderId = Guid.NewGuid(), StoreId = storeId, Status = "Completed" }
            };
            _mockOrderService.Setup(s => s.GetByStoreIdAsync(storeId)).ReturnsAsync(orders);

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("sellerId", "Seller") }
            };

            // Act
            var result = await _orderController.GetByStoreId(storeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnOrders.Count());
        }

        [Fact]
        public async Task GetByUserId_OrdersExistWithUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = "buyerId";
            var orders = new List<OrderDto>
            {
                new OrderDto { OrderId = Guid.NewGuid(), UserId = userId, Status = "Pending" },
                new OrderDto { OrderId = Guid.NewGuid(), UserId = userId, Status = "Completed" }
            };
            _mockOrderService.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(orders);

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal(userId, "Buyer") }
            };

            // Act
            var result = await _orderController.GetByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Equal(2, returnOrders.Count());
        }

        [Fact]
        public async Task Create_ValidOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var orderCreateDto = new OrderCreateDto { StoreId = Guid.NewGuid(), OrderItems = new List<OrderItemCreateDto> { new OrderItemCreateDto { ProductId = Guid.NewGuid(), Quantity = 1 } } };
            var createdOrder = new OrderDto { OrderId = Guid.NewGuid(), Status = "Pending" };

            _mockOrderService.Setup(s => s.AddAsync(orderCreateDto, It.IsAny<string>())).ReturnsAsync(createdOrder);

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("buyerId", "Buyer") }
            };

            // Act
            var result = await _orderController.Create(orderCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(createdOrder.OrderId, ((OrderDto)createdAtActionResult.Value).OrderId);
        }

        [Fact]
        public async Task UpdateStatus_ValidOrder_ReturnsNoContentResult()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var status = "Completed";

            _orderController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("sellerId", "Seller") }
            };

            // Act
            var result = await _orderController.UpdateStatus(orderId, status);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}