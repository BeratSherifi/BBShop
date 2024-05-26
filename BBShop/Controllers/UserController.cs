using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBShop.Controllers;
// Controllers/UserController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Add(UserCreateDto userDto)
    {
        await _userService.AddAsync(userDto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UserUpdateDto userDto)
    {
        try
        {
            await _userService.UpdateAsync(id, userDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}
