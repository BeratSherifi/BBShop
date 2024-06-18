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
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _userController = new UserController(_mockUserService.Object);
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
        public async Task GetAll_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<UserDto> { new UserDto { Id = "1", Username = "testuser" } };
            _mockUserService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Single(returnUsers);
        }

        [Fact]
        public async Task GetById_UserExists_ReturnsOkResult()
        {
            // Arrange
            var user = new UserDto { Id = "1", Username = "testuser" };
            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(user);

            // Act
            var result = await _userController.GetById("1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("1", returnUser.Id);
        }

        [Fact]
        public async Task GetById_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync((UserDto)null);

            // Act
            var result = await _userController.GetById("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_ValidUser_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userCreateDto = new UserCreateDto { Username = "testuser", Email = "test@example.com", Password = "password", FullName = "Test User", Role = "User" };
            _mockUserService.Setup(s => s.AddAsync(userCreateDto)).ReturnsAsync("1");

            // Act
            var result = await _userController.Add(userCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("1", createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_UserExistsAndAuthorized_ReturnsNoContentResult()
        {
            // Arrange
            var userUpdateDto = new UserUpdateDto { Username = "updateduser", Email = "updated@example.com" };
            var userDto = new UserDto { Id = "1", Username = "testuser" };

            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(userDto);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("1", "User") }
            };

            // Act
            var result = await _userController.Update("1", userUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync((UserDto)null);

            // Act
            var result = await _userController.Update("1", new UserUpdateDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_UserNotAuthorized_ReturnsForbidResult()
        {
            // Arrange
            var userUpdateDto = new UserUpdateDto { Username = "updateduser", Email = "updated@example.com" };
            var userDto = new UserDto { Id = "1", Username = "testuser" };

            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(userDto);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("2", "User") }
            };

            // Act
            var result = await _userController.Update("1", userUpdateDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_UserExistsAndAuthorized_ReturnsNoContentResult()
        {
            // Arrange
            var userDto = new UserDto { Id = "1", Username = "testuser" };

            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(userDto);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("1", "User") }
            };

            // Act
            var result = await _userController.Delete("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_UserDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync((UserDto)null);

            // Act
            var result = await _userController.Delete("1");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_UserNotAuthorized_ReturnsForbidResult()
        {
            // Arrange
            var userDto = new UserDto { Id = "1", Username = "testuser" };

            _mockUserService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(userDto);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("2", "User") }
            };

            // Act
            var result = await _userController.Delete("1");

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}