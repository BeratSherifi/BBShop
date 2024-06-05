using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Repositories
{
    public class UserRepositoryTest
    {
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_User")
                .Options;

            _context = new AppDbContext(options);
            _userRepository = new UserRepository(_context);

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();

            var users = new List<User>
            {
                new User { Id = "user1", UserName = "user1@example.com", FullName = "User One", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new User { Id = "user2", UserName = "user2@example.com", FullName = "User Two", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            SeedDatabase();
            // Arrange
            var userId = _context.Users.First().Id;

            // Act
            var result = await _userRepository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            SeedDatabase();
            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            SeedDatabase();
            // Arrange
            var newUser = new User { Id = "user3", UserName = "user3@example.com", FullName = "User Three", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now };

            // Act
            await _userRepository.AddAsync(newUser);

            // Assert
            Assert.Equal(3, _context.Users.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            SeedDatabase();
            // Arrange
            var user = _context.Users.First();
            user.FullName = "Updated User";

            // Act
            await _userRepository.UpdateAsync(user);

            // Assert
            var updatedUser = _context.Users.First();
            Assert.Equal("Updated User", updatedUser.FullName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            SeedDatabase();
            // Arrange
            var userId = _context.Users.First().Id;
            var user = await _userRepository.GetByIdAsync(userId);

            // Act
            if (user != null)
            {
                await _userRepository.DeleteAsync(userId);
            }

            // Assert
            Assert.Equal(1, _context.Users.Count());
        }
    }
}
