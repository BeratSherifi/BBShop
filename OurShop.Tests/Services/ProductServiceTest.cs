using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Implementations;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Hosting;

namespace OurShop.Tests.Services
{
    public class ProductServiceTest
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IStoreRepository> _storeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _storeRepositoryMock = new Mock<IStoreRepository>();
            _mapperMock = new Mock<IMapper>();
            var environmentMock = new Mock<IWebHostEnvironment>();
            _productService = new ProductService(_productRepositoryMock.Object, _storeRepositoryMock.Object, environmentMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId };
            var productDto = new ProductDto { ProductId = productId };
            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _productService.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            var productDtos = new List<ProductDto> { new ProductDto(), new ProductDto() };
            _productRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ShouldReturnProducts_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var products = new List<Product> { new Product { Store = new Store { StoreName = storeName } } };
            var productDtos = new List<ProductDto> { new ProductDto { StoreName = storeName } };
            _productRepositoryMock.Setup(repo => repo.GetByStoreNameAsync(storeName)).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetByStoreNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Equal(storeName, result.First().StoreName);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            // Arrange
            var productCreateDto = new ProductCreateDto
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 100,
                StockQuantity = 10
            };
            var store = new Store { StoreId = Guid.NewGuid() };
            var product = new Product { StoreId = store.StoreId };
            var userId = "user1";

            _storeRepositoryMock.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(store);
            _mapperMock.Setup(m => m.Map<Product>(productCreateDto)).Returns(product);
            _productRepositoryMock.Setup(repo => repo.AddAsync(product)).Returns(Task.CompletedTask);

            // Act
            await _productService.AddAsync(productCreateDto, userId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.AddAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productUpdateDto = new ProductUpdateDto
            {
                ProductName = "Updated Product",
                Description = "Updated Description",
                Price = 200,
                StockQuantity = 20
            };
            var product = new Product { ProductId = productId, UserId = "user1" };
            var userId = "user1";

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map(productUpdateDto, product)).Returns(product);

            // Act
            await _productService.UpdateAsync(productId, productUpdateDto, userId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, UserId = "user1" };
            var userId = "user1";

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _productRepositoryMock.Setup(repo => repo.DeleteAsync(product)).Returns(Task.CompletedTask);

            // Act
            await _productService.DeleteAsync(productId, userId);

            // Assert
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(product), Times.Once);
        }
    }
}
