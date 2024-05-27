using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BBShop.Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly UserManager<User> _userManager; 
        private readonly IMapper _mapper;

        public StoreService(IStoreRepository storeRepository, UserManager<User> userManager, IMapper mapper)
        {
            _storeRepository = storeRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<StoreDto> GetByIdAsync(Guid id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            return _mapper.Map<StoreDto>(store);
        }

        public async Task<StoreDto> AddAsync(StoreCreateDto storeDto, string userId)
        {
            if (!await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), "Seller"))
            {
                throw new Exception("Only sellers can create stores.");
            }

            var store = _mapper.Map<Store>(storeDto);
            store.UserId = userId;
            store.CreatedAt = DateTime.UtcNow;
            store.UpdatedAt = DateTime.UtcNow;
            await _storeRepository.AddAsync(store);
    
            return _mapper.Map<StoreDto>(store); // Return the created store with StoreId
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

        public async Task<IEnumerable<StoreDto>> SearchByNameAsync(string name)
        {
            var stores = await _storeRepository.SearchByNameAsync(name);
            return _mapper.Map<IEnumerable<StoreDto>>(stores);
        }
    }
}
