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
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        public StoreService(IStoreRepository storeRepository, IWebHostEnvironment environment, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _environment = environment;
            _mapper = mapper;
        }

        public async Task<StoreDto> GetByIdAsync(Guid id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            return _mapper.Map<StoreDto>(store);
        }

        public async Task<StoreDto> AddAsync(StoreCreateDto storeDto, string userId)
        {
            var store = _mapper.Map<Store>(storeDto);
            store.UserId = userId;
            store.CreatedAt = DateTime.UtcNow;
            store.UpdatedAt = DateTime.UtcNow;
            store.LogoUrl = SaveFile(storeDto.Logo);
            await _storeRepository.AddAsync(store);
            return _mapper.Map<StoreDto>(store);
        }

        public async Task UpdateAsync(Guid id, StoreUpdateDto storeDto)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
            {
                throw new Exception("Store not found.");
            }

            _mapper.Map(storeDto, store);
            store.UpdatedAt = DateTime.UtcNow;
            store.LogoUrl = SaveFile(storeDto.Logo);
            await _storeRepository.UpdateAsync(store);
        }

        public async Task DeleteAsync(Guid id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
            {
                throw new Exception("Store not found.");
            }

            await _storeRepository.DeleteAsync(store);
        }

        public async Task<IEnumerable<StoreDto>> SearchStoresAsync(string query)
        {
            var stores = await _storeRepository.SearchByNameAsync(query);
            return _mapper.Map<IEnumerable<StoreDto>>(stores);
        }

        private string SaveFile(IFormFile file)
        {
            if (file == null) return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "stores");
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return $"/uploads/stores/{Path.GetFileName(filePath)}";
        }



        public async Task<StoreDto> GetByUserId(string userId)
        {
            var store = await _storeRepository.GetByUserIdAsync(userId);
            return _mapper.Map<StoreDto>(store);
        }
    }
}
