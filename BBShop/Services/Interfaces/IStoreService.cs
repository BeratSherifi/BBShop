using BBShop.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BBShop.Models;

namespace BBShop.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreDto> GetByIdAsync(Guid id);
        Task<StoreDto> AddAsync(StoreCreateDto storeDto, string userId);
        Task UpdateAsync(Guid id, StoreUpdateDto storeDto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<StoreDto>> SearchStoresAsync(string query); 
        Task<StoreDto> GetByUserId(string userId);
    }
}