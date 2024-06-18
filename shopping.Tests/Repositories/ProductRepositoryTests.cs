using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace shopping.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);
            _productRepository = new ProductRepository(_context);
        }

        private void ClearDatabase()
        {
            _context.Products.RemoveRange(_context.Products);
            _context.Stores.RemoveRange(_context.Stores);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsProduct()
        {
            // Arrange
            ClearDatabase();
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, ProductName = "Test Product", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Test Product", result.ProductName);
        }

        [Fact]
        public async Task GetAllAsync_ProductsExist_ReturnsAllProducts()
        {
            // Arrange
            ClearDatabase();
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() }
            };
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductName == "Product 1");
            Assert.Contains(result, p => p.ProductName == "Product 2");
        }

        [Fact]
        public async Task AddAsync_SuccessfulCreation_AddsProduct()
        {
            // Arrange
            ClearDatabase();
            var product = new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() };

            // Act
            await _productRepository.AddAsync(product);
            var result = await _context.Products.FindAsync(product.ProductId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductId, result.ProductId);
            Assert.Equal("Test Product", result.ProductName);
        }

        [Fact]
        public async Task UpdateAsync_ProductExists_UpdatesProduct()
        {
            // Arrange
            ClearDatabase();
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, ProductName = "Test Product", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            product.ProductName = "Updated Product";
            await _productRepository.UpdateAsync(product);
            var result = await _context.Products.FindAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Updated Product", result.ProductName);
        }

        [Fact]
        public async Task DeleteAsync_ProductExists_DeletesProduct()
        {
            // Arrange
            ClearDatabase();
            var productId = Guid.NewGuid();
            var product = new Product { ProductId = productId, ProductName = "Test Product", Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId", StoreId = Guid.NewGuid() };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            await _productRepository.DeleteAsync(product);
            var result = await _context.Products.FindAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByStoreNameAsync_ProductsExistWithStoreName_ReturnsProducts()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var store = new Store { StoreId = storeId, StoreName = "Test Store", UserId = "UserId", LogoUrl = "LogoUrl" };
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1", StoreId = storeId, Store = store, Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2", StoreId = storeId, Store = store, Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId" }
            };
            _context.Stores.Add(store);
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetByStoreNameAsync("Test Store");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductName == "Product 1");
            Assert.Contains(result, p => p.ProductName == "Product 2");
        }

        [Fact]
        public async Task GetByStoreIdAsync_ProductsExistWithStoreId_ReturnsProducts()
        {
            // Arrange
            ClearDatabase();
            var storeId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 1", StoreId = storeId, Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Product 2", StoreId = storeId, Description = "Description", ImageUrl = "ImageUrl", UserId = "UserId" }
            };
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetByStoreIdAsync(storeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductName == "Product 1");
            Assert.Contains(result, p => p.ProductName == "Product 2");
        }
    }
}
