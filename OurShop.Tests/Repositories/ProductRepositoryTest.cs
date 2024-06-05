using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OurShop.Tests.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly Mock<DbSet<Product>> _mockSet;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTest()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockSet = new Mock<DbSet<Product>>();
            _productRepository = new ProductRepository(_mockContext.Object);
        }

        private void SetupMockDbSet(IQueryable<Product> products)
        {
            _mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            _mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            _mockContext.Setup(c => c.Products).Returns(_mockSet.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { ProductId = productId, ProductName = "Test Product 1", Price = 100, StoreId = Guid.NewGuid(), UserId = "user1" }
            }.AsQueryable();

            SetupMockDbSet(products);

            // Act
            var result = await _productRepository.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 1", Price = 100, StoreId = Guid.NewGuid(), UserId = "user1" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 2", Price = 200, StoreId = Guid.NewGuid(), UserId = "user2" }
            }.AsQueryable();

            SetupMockDbSet(products);

            // Act
            var result = await _productRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ShouldReturnProducts_WhenStoreNameMatches()
        {
            // Arrange
            var storeName = "Test Store";
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 1", Price = 100, Store = new Store { StoreName = storeName }, UserId = "user1" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 2", Price = 200, Store = new Store { StoreName = "Another Store" }, UserId = "user2" }
            }.AsQueryable();

            SetupMockDbSet(products);

            // Act
            var result = await _productRepository.GetByStoreNameAsync(storeName);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, p => p.Store.StoreName == storeName);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            // Arrange
            var newProduct = new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 3", Price = 300, StoreId = Guid.NewGuid(), UserId = "user3" };

            // Act
            await _productRepository.AddAsync(newProduct);

            // Assert
            _mockSet.Verify(m => m.AddAsync(newProduct, default), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 1", Price = 100, StoreId = Guid.NewGuid(), UserId = "user1" };

            // Act
            product.ProductName = "Updated Product";
            await _productRepository.UpdateAsync(product);

            // Assert
            _mockSet.Verify(m => m.Update(product), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, ProductName = "Test Product 1", Price = 100, StoreId = Guid.NewGuid(), UserId = "user1" };
            var products = new List<Product> { product }.AsQueryable();

            SetupMockDbSet(products);

            // Act
            await _productRepository.DeleteAsync(product);

            // Assert
            _mockSet.Verify(m => m.Remove(product), Times.Once());
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
        }
    }
}
