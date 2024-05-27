

using System.Security.Claims;
using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOrSellerOrBuyerPolicy")] // everyone can access
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSellerOrBuyerPolicy")] // only admin, seller, and buyer can access
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous] // everyone can create account
        public async Task<IActionResult> Add(UserCreateDto userDto)
        {
            var userId = await _userService.AddAsync(userDto);
            return CreatedAtAction(nameof(GetById), new { id = userId }, userId);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSellerOrBuyerPolicy")] // only admin, seller, and buyer can access
        public async Task<IActionResult> Update(string id, UserUpdateDto userUpdateDto)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to update the account
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || (!User.IsInRole("Admin") && currentUserId != id))
            {
                return Forbid();
            }

            await _userService.UpdateAsync(id, userUpdateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSellerOrBuyerPolicy")] // only admin, seller, and buyer can access
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is authorized to delete the account
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null || (!User.IsInRole("Admin") && currentUserId != id))
            {
                return Forbid();
            }

            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}