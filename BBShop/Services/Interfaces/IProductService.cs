using BBShop.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetByStoreNameAsync(string storeName);
        Task AddAsync(ProductCreateDto productDto, string userId);
        Task UpdateAsync(Guid productId, ProductUpdateDto productDto, string userId);
        Task DeleteAsync(Guid productId, string userId);
    }
}