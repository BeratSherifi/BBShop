using System.Threading.Tasks;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace OurShop.Tests.Controllers
{
    public class AuthControllerTest
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTest()
        {
            _userManagerMock = MockUserManager();
            _configurationMock = new Mock<IConfiguration>();
            _tokenServiceMock = new Mock<TokenService>(null, null) { CallBase = true };
            _authController = new AuthController(_userManagerMock.Object, _configurationMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "password" };
            var user = new User { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(true);
            _tokenServiceMock.Setup(ts => ts.CreateToken(It.IsAny<User>())).ReturnsAsync("mockToken");

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenResult = Assert.IsType<dynamic>(okResult.Value);
            Assert.Equal("mockToken", tokenResult.token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "wrongpassword" };
            var user = new User { UserName = "testuser", Email = "test@example.com" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginDto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password)).ReturnsAsync(false);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            return mgr;
        }
    }
}
