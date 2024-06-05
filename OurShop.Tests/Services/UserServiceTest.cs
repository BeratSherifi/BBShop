using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();
            _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object, _userManagerMock.Object, _roleManagerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "user1";
            var user = new User { Id = userId };
            var userDto = new UserDto { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("User", result.Role);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User> { new User { Id = "user1" }, new User { Id = "user2" } };
            var userDtos = new List<UserDto> { new UserDto { Id = "user1" }, new UserDto { Id = "user2" } };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(userDtos);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(users[0]);
            _userManagerMock.Setup(um => um.FindByIdAsync("user2")).ReturnsAsync(users[1]);
            _userManagerMock.Setup(um => um.GetRolesAsync(users[0])).ReturnsAsync(new List<string> { "User" });
            _userManagerMock.Setup(um => um.GetRolesAsync(users[1])).ReturnsAsync(new List<string> { "Admin" });

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("User", result.First().Role);
            Assert.Equal("Admin", result.Last().Role);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var userCreateDto = new UserCreateDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                FullName = "Test User",
                Role = "User"
            };
            var user = new User { UserName = userCreateDto.Username, Email = userCreateDto.Email, FullName = userCreateDto.FullName };
            _mapperMock.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
            _userManagerMock.Setup(um => um.CreateAsync(user, userCreateDto.Password)).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(userCreateDto.Role)).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, userCreateDto.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.AddAsync(userCreateDto);

            // Assert
            Assert.NotNull(result);
            _userManagerMock.Verify(um => um.CreateAsync(user, userCreateDto.Password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(user, userCreateDto.Role), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var userId = "user1";
            var userUpdateDto = new UserUpdateDto
            {
                Username = "updateduser",
                Email = "updated@example.com"
            };
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _userService.UpdateAsync(userId, userUpdateDto);

            // Assert
            _userManagerMock.Verify(um => um.UpdateAsync(It.Is<User>(u => u.UserName == userUpdateDto.Username && u.Email == userUpdateDto.Email)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var userId = "user1";

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(userId), Times.Once);
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }

        private Mock<RoleManager<IdentityRole>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            var mgr = new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
            return mgr;
        }
    }
}
