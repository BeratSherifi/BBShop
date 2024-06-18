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

       


    }
}
