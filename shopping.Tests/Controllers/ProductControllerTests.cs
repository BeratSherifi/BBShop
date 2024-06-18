using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BBShop.Controllers;
using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace shopping.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _productController = new ProductController(_mockProductService.Object);
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
        public async Task GetById_ProductExists_ReturnsOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDto { ProductId = productId, ProductName = "Test Product" };
            _mockProductService.Setup(s => s.GetByIdAsync(productId)).ReturnsAsync(productDto);

            // Act
            var result = await _productController.GetById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnProduct.ProductId);
        }

        [Fact]
        public async Task GetById_ProductDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProductDto)null);

            // Act
            var result = await _productController.GetById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAll_ProductsExist_ReturnsOkResult()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 1" },
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 2" }
            };
            _mockProductService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetByStoreName_ProductsExistWithStoreName_ReturnsOkResult()
        {
            // Arrange
            var storeName = "Test Store";
            var products = new List<ProductDto>
            {
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 1", StoreName = storeName },
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 2", StoreName = storeName }
            };
            _mockProductService.Setup(s => s.GetByStoreNameAsync(storeName)).ReturnsAsync(products);

            // Act
            var result = await _productController.GetByStoreName(storeName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task GetByStoreId_ProductsExistWithStoreId_ReturnsOkResult()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var products = new List<ProductDto>
            {
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 1" },
                new ProductDto { ProductId = Guid.NewGuid(), ProductName = "Product 2" }
            };
            _mockProductService.Setup(s => s.GetByStoreIdAsync(storeId)).ReturnsAsync(products);

            // Act
            var result = await _productController.GetByStoreId(storeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, returnProducts.Count());
        }

        [Fact]
        public async Task Add_ValidProduct_ReturnsOkResult()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto { ProductName = "Test Product" };
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _productController.Add(productCreateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_ValidProduct_ReturnsNoContentResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductName = "Updated Product" };
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _productController.Update(productId, productUpdateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_InvalidProduct_ReturnsBadRequestResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto { ProductName = "Updated Product" };
            _mockProductService.Setup(s => s.UpdateAsync(productId, productUpdateDto, "userId"))
                .ThrowsAsync(new Exception("Product not found"));
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _productController.Update(productId, productUpdateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product not found", badRequestResult.Value);
        }

        [Fact]
        public async Task Delete_ValidProduct_ReturnsNoContentResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _productController.Delete(productId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_InvalidProduct_ReturnsBadRequestResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(s => s.DeleteAsync(productId, "userId"))
                .ThrowsAsync(new Exception("Product not found"));
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateClaimsPrincipal("userId", "Seller") }
            };

            // Act
            var result = await _productController.Delete(productId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product not found", badRequestResult.Value);
        }
    }
}
