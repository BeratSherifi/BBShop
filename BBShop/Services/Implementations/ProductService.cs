using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BBShop.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IStoreRepository storeRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _environment = environment;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetByStoreNameAsync(string storeName)
        {
            var products = await _productRepository.GetByStoreNameAsync(storeName);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task AddAsync(ProductCreateDto productDto, string userId)
        {
            var store = await _storeRepository.GetByUserIdAsync(userId);
            if (store == null)
            {
                throw new Exception("Store not found for the user.");
            }

            var product = _mapper.Map<Product>(productDto);
            product.StoreId = store.StoreId;
            product.UserId = userId;
            product.ImageUrl = SaveFile(productDto.Image);
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateAsync(Guid productId, ProductUpdateDto productDto, string userId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.UserId != userId)
            {
                throw new Exception("Product not found or user not authorized.");
            }

            _mapper.Map(productDto, product);
            product.ImageUrl = SaveFile(productDto.Image);
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteAsync(Guid productId, string userId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.UserId != userId)
            {
                throw new Exception("Product not found or user not authorized.");
            }

            await _productRepository.DeleteAsync(product);
        }

        private string SaveFile(IFormFile file)
        {
            if (file == null) return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return filePath;
        }
    }
}
