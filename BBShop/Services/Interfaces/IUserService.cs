

using BBShop.DTOs;

namespace BBShop.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(string id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task AddAsync(UserCreateDto userDto);
    Task UpdateAsync(string id, UserUpdateDto userDto);
    Task DeleteAsync(string id);
}



