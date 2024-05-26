using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BBShop.Services.Implementations;

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

    public async Task<IEnumerable<StoreDto>> GetAllAsync()
    {
        var stores = await _storeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StoreDto>>(stores);
    }

    public async Task AddAsync(StoreCreateDto storeDto)
    {
        var user = await _userManager.FindByIdAsync(storeDto.UserId);

        if (user == null)
        {
            throw new Exception("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains("seller"))
        {
            throw new Exception("Only users with the 'seller' role can create a store");
        }

        var existingStore = await _storeRepository.GetAllAsync();
        if (existingStore.Any(s => s.UserId == storeDto.UserId))
        {
            throw new Exception("A seller can only have one store");
        }

        var store = _mapper.Map<Store>(storeDto);
        await _storeRepository.AddAsync(store);
    }

    public async Task UpdateAsync(Guid storeId, StoreUpdateDto storeDto)
    {
        var store = await _storeRepository.GetByIdAsync(storeId);
        if (store == null)
        {
            throw new Exception("Store not found");
        }

        store.StoreName = storeDto.StoreName;
        store.CreatedAt = storeDto.CreatedAt;
        store.UpdatedAt = storeDto.UpdatedAt;

        await _storeRepository.UpdateAsync(store);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _storeRepository.DeleteAsync(id);
    }
}