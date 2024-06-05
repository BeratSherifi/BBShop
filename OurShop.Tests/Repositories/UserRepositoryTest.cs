using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Repositories
{
    public class UserRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<DbSet<User>> _mockSet;
        private readonly UserRepository _userRepository;

        public UserRepositoryTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockSet = new Mock<DbSet<User>>();
            _userRepository = new UserRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<User> users)
        {
            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            _mockContext.Setup(c => c.Users).Returns(_mockSet.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "user1";
            var users = new List<User>
            {
                new User { Id = userId, FullName = "Test User 1", CreatedAt = DateTime.Now }
            }.AsQueryable();

            SetupMockDbSet(users);

            // Act
            var result = await _userRepository.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "user1", FullName = "Test User 1", CreatedAt = DateTime.Now },
                new User { Id = "user2", FullName = "Test User 2", CreatedAt = DateTime.Now }
            }.AsQueryable();

            SetupMockDbSet(users);

            // Act
            var result = await _userRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddUser()
        {
            // Arrange
            var newUser = new User { Id = "user3", FullName = "Test User 3", CreatedAt = DateTime.Now };

            // Act
            await _userRepository.AddAsync(newUser);

            // Assert
            _mockSet.Verify(m => m.AddAsync(newUser, default), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var user = new User { Id = "user1", FullName = "Test User 1", CreatedAt = DateTime.Now };

            // Act
            user.FullName = "Updated User";
            await _userRepository.UpdateAsync(user);

            // Assert
            _mockSet.Verify(m => m.Update(user), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            // Arrange
            var userId = "user1";
            var user = new User { Id = userId, FullName = "Test User 1", CreatedAt = DateTime.Now };
            var users = new List<User> { user }.AsQueryable();

            SetupMockDbSet(users);

            // Act
            await _userRepository.DeleteAsync(userId);

            // Assert
            _mockSet.Verify(m => m.Remove(user), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}
