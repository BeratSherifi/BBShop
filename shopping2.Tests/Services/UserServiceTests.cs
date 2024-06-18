using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace shopping2.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();

            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _userService = new UserService(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_UserExists_ReturnsUserDto()
        {
            // Arrange
            var userId = "1";
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
            var userDto = new UserDto { Id = userId, Username = "testuser", Email = "test@example.com", Role = "User" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.Id, result.Id);
            Assert.Equal(userDto.Username, result.Username);
            Assert.Equal(userDto.Email, result.Email);
            Assert.Equal(userDto.Role, result.Role);
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_ReturnsUserId()
        {
            // Arrange
            var userCreateDto = new UserCreateDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                FullName = "New User",
                Role = "User"
            };
            var user = new User { UserName = userCreateDto.Username, Email = userCreateDto.Email, FullName = userCreateDto.FullName };

            _mockMapper.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
            _mockUserManager.Setup(um => um.CreateAsync(user, userCreateDto.Password)).ReturnsAsync(IdentityResult.Success);
            _mockRoleManager.Setup(rm => rm.RoleExistsAsync(userCreateDto.Role)).ReturnsAsync(true);
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, userCreateDto.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.AddAsync(userCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task GetAllAsync_UsersExist_ReturnsUserDtos()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1", Email = "user1@example.com" },
                new User { Id = "2", UserName = "user2", Email = "user2@example.com" }
            };
            var userDtos = new List<UserDto>
            {
                new UserDto { Id = "1", Username = "user1", Email = "user1@example.com", Role = "User" },
                new UserDto { Id = "2", Username = "user2", Email = "user2@example.com", Role = "User" }
            };

            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);

            foreach (var user in users)
            {
                _mockUserManager.Setup(um => um.FindByIdAsync(user.Id)).ReturnsAsync(user);
                _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            }

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDtos.Count, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_UserExists_UpdatesUser()
        {
            // Arrange
            var userId = "1";
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
            var userUpdateDto = new UserUpdateDto { Username = "updateduser", Email = "updated@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.UpdateAsync(userId, userUpdateDto);

            // Assert
            _mockUserManager.Verify(um => um.UpdateAsync(It.Is<User>(u => u.UserName == userUpdateDto.Username && u.Email == userUpdateDto.Email)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_UserExists_DeletesUser()
        {
            // Arrange
            var userId = "1";
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.DeleteAsync(userId)).Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }
    }
}