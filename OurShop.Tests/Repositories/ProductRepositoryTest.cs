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
    public class ProductRepositoryTest
    {
        private readonly ProductRepository _productRepository;
        private readonly AppDbContext _context;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_Product")
                .Options;

            _context = new AppDbContext(options);
            _productRepository = new ProductRepository(_context);

            // Ensure the database is clean before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _context.Products.RemoveRange(_context.Products);
            _context.Stores.RemoveRange(_context.Stores);
            _context.SaveChanges();

            var store1 = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 1", UserId = "user1", LogoUrl = "logo1.png", CreatedAt = DateTime.Now };
            var store2 = new Store { StoreId = Guid.NewGuid(), StoreName = "Test Store 2", UserId = "user2", LogoUrl = "logo2.png", CreatedAt = DateTime.Now };
            _context.Stores.AddRange(store1, store2);
            _context.SaveChanges();

            var products = new List<Product>
            {
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 1", Description = "Description 1", Price = 100, StockQuantity = 10, StoreId = store1.StoreId, UserId = "user1", ImageUrl = "image1.png" },
                new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 2", Description = "Description 2", Price = 200, StockQuantity = 20, StoreId = store2.StoreId, UserId = "user2", ImageUrl = "image2.png" }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            SeedDatabase();
            // Arrange
            var productId = _context.Products.First().ProductId;

            // Act
            var result = await _productRepository.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            SeedDatabase();
            // Act
            var result = await _productRepository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByStoreNameAsync_ShouldReturnProducts_WhenStoreNameMatches()
        {
            SeedDatabase();
            // Arrange
            var storeName = "Test Store 1";
            var storeId = _context.Stores.First(s => s.StoreName == storeName).StoreId;
            _context.Products.Add(new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 3", Description = "Description 3", Price = 300, StockQuantity = 30, StoreId = storeId, UserId = "user3", ImageUrl = "image3.png" });
            _context.SaveChanges();

            // Act
            var result = await _productRepository.GetByStoreNameAsync(storeName);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddProduct()
        {
            SeedDatabase();
            // Arrange
            var newProduct = new Product { ProductId = Guid.NewGuid(), ProductName = "Test Product 3", Description = "Description 3", Price = 300, StockQuantity = 30, StoreId = _context.Stores.First().StoreId, UserId = "user3", ImageUrl = "image3.png" };

            // Act
            await _productRepository.AddAsync(newProduct);

            // Assert
            Assert.Equal(3, _context.Products.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct()
        {
            SeedDatabase();
            // Arrange
            var product = _context.Products.First();
            product.ProductName = "Updated Product";

            // Act
            await _productRepository.UpdateAsync(product);

            // Assert
            var updatedProduct = _context.Products.First();
            Assert.Equal("Updated Product", updatedProduct.ProductName);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            SeedDatabase();
            // Arrange
            var productId = _context.Products.First().ProductId;
            var product = await _productRepository.GetByIdAsync(productId);

            // Act
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }

            // Assert
            Assert.Equal(2, _context.Products.Count());
        }
    }
}
