using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace shopping.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockMapper = new Mock<IMapper>();
            _productService = new ProductService(_mockProductRepository.Object, _mockStoreRepository.Object, _mockEnvironment.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsProductDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, ProductName = "Test Product" };
            var productDto = new ProductDto { ProductId = productId, ProductName = "Test Product" };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDto.ProductId, result.ProductId);
            Assert.Equal(productDto.ProductName, result.ProductName);
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_ReturnsProductDto()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductName = "New Product",
                Description = "Product Description",
                Price = 10.99m,
                StockQuantity = 100,
                Image = new Mock<IFormFile>().Object // Mock image file
            };
            var userId = "123";
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, UserId = userId };
            var product = new Product { ProductId = Guid.NewGuid(), ProductName = productCreateDto.ProductName, StoreId = storeId, UserId = userId };
            var productDto = new ProductDto { ProductId = product.ProductId, ProductName = product.ProductName };

            _mockStoreRepository.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(store);
            _mockMapper.Setup(m => m.Map<Product>(productCreateDto)).Returns(product);
            _mockProductRepository.Setup(repo => repo.AddAsync(product)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
            _mockEnvironment.Setup(env => env.WebRootPath).Returns("wwwroot");

            // Act
            await _productService.AddAsync(productCreateDto, userId);

            // Assert
            _mockProductRepository.Verify(repo => repo.AddAsync(It.Is<Product>(p => p.ProductName == productCreateDto.ProductName && p.ImageUrl != null)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ProductExists_DeletesProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var userId = "123";
            var product = new Product { ProductId = productId, ProductName = "Test Product", UserId = userId };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.DeleteAsync(product)).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteAsync(productId, userId);

            // Assert
            _mockProductRepository.Verify(repo => repo.DeleteAsync(product), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ProductsExist_ReturnsProductDtos()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2" }
            };
            var productDtos = products.Select(p => new ProductDto { ProductId = p.ProductId, ProductName = p.ProductName });

            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDtos.Count(), result.Count());
        }

        [Fact]
        public async Task GetByStoreIdAsync_ProductsExist_ReturnsProductDtos()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1", StoreId = storeId },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2", StoreId = storeId }
            };
            var productDtos = products.Select(p => new ProductDto { ProductId = p.ProductId, ProductName = p.ProductName });

            _mockProductRepository.Setup(repo => repo.GetByStoreIdAsync(storeId)).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetByStoreIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDtos.Count(), result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ProductsExist_ReturnsProductDtos()
        {
            // Arrange
            var storeName = "Test Store";
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1", Store = new Store { StoreName = storeName } },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2", Store = new Store { StoreName = storeName } }
            };
            var productDtos = products.Select(p => new ProductDto { ProductId = p.ProductId, ProductName = p.ProductName });

            _mockProductRepository.Setup(repo => repo.GetByStoreNameAsync(storeName)).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetByStoreNameAsync(storeName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productDtos.Count(), result.Count());
        }
    }
}
