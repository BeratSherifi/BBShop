using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace shopping2.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        private void ClearDatabase()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            ClearDatabase();
            var userId = "123";
            var user = new User { Id = userId, UserName = "testuser", FullName = "Test User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.UserName);
        }

        [Fact]
        public async Task GetAllAsync_UsersExist_ReturnsAllUsers()
        {
            // Arrange
            ClearDatabase();
            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1", FullName = "User One" },
                new User { Id = "2", UserName = "user2", FullName = "User Two" }
            };
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.UserName == "user1");
            Assert.Contains(result, u => u.UserName == "user2");
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_AddsUser()
        {
            // Arrange
            ClearDatabase();
            var user = new User { Id = "123", UserName = "testuser", FullName = "Test User" };

            // Act
            await _userRepository.AddAsync(user);
            var result = await _context.Users.FindAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("123", result.Id);
            Assert.Equal("testuser", result.UserName);
        }

        [Fact]
        public async Task UpdateAsync_UserExists_UpdatesUser()
        {
            // Arrange
            ClearDatabase();
            var userId = "123";
            var user = new User { Id = userId, UserName = "testuser", FullName = "Test User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.UserName = "updateduser";
            await _userRepository.UpdateAsync(user);
            var result = await _context.Users.FindAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("updateduser", result.UserName);
        }

        [Fact]
        public async Task DeleteAsync_UserExists_DeletesUser()
        {
            // Arrange
            ClearDatabase();
            var userId = "123";
            var user = new User { Id = userId, UserName = "testuser", FullName = "Test User" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _userRepository.DeleteAsync(userId);
            var result = await _context.Users.FindAsync(userId);

            // Assert
            Assert.Null(result);
        }
    }
}