
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBShop.Services.Implementations;


using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(IUserRepository userRepository, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<UserDto> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Role = roles.FirstOrDefault(); // Assuming a user has a single role

        return userDto;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        foreach (var userDto in userDtos)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            var roles = await _userManager.GetRolesAsync(user);
            userDto.Role = roles.FirstOrDefault(); // Assuming a user has a single role
        }

        return userDtos;
    }

    public async Task AddAsync(UserCreateDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        var result = await _userManager.CreateAsync(user, userDto.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Ensure the role exists
        if (!await _roleManager.RoleExistsAsync(userDto.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole(userDto.Role));
        }

        await _userManager.AddToRoleAsync(user, userDto.Role);
    }

    public async Task UpdateAsync(string id, UserUpdateDto userDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        user.UserName = userDto.Username;
        user.Email = userDto.Email;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Update user roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, userDto.Role);
    }


    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
    }
}


