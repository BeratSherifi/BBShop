using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace OurShop.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<UserDto> { new UserDto { Id = "user1" }, new UserDto { Id = "user2" } };
            _userServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WithUser()
        {
            // Arrange
            var userId = "user1";
            var user = new UserDto { Id = userId };
            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userController.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "user1";
            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _userController.GetById(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Add_ShouldReturnCreatedAtActionResult_WithUserId()
        {
            // Arrange
            var userCreateDto = new UserCreateDto { Username = "testuser", Email = "test@example.com", Password = "Password123!", FullName = "Test User", Role = "User" };
            var userId = "user1";
            _userServiceMock.Setup(service => service.AddAsync(userCreateDto)).ReturnsAsync(userId);

            // Act
            var result = await _userController.Add(userCreateDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_userController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(userId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(userId, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenUserIsUpdated()
        {
            // Arrange
            var userId = "user1";
            var userUpdateDto = new UserUpdateDto { Username = "updateduser", Email = "updated@example.com" };
            var user = new UserDto { Id = userId };

            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _userController.Update(userId, userUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var userId = "user1";
            var userUpdateDto = new UserUpdateDto { Username = "updateduser", Email = "updated@example.com" };
            var user = new UserDto { Id = userId };

            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "anotherUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _userController.Update(userId, userUpdateDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenUserIsDeleted()
        {
            // Arrange
            var userId = "user1";
            var user = new UserDto { Id = userId };

            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var userId = "user1";
            var user = new UserDto { Id = userId };

            _userServiceMock.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "anotherUser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            _userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}
