using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BBShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOrSellerOrBuyerPolicy")] // Admin, Seller, and Buyer can access
        public async Task<IActionResult> GetById(Guid id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrSellerPolicy")]
        public async Task<IActionResult> Create([FromForm] StoreCreateDto storeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdStore = await _storeService.AddAsync(storeDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdStore.StoreId }, createdStore);
        }



        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSellerPolicy")] // Only Admin and Seller can access
        public async Task<IActionResult> Update(Guid id, [FromForm] StoreUpdateDto storeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();

            if (store.UserId != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            await _storeService.UpdateAsync(id, storeDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSellerPolicy")] // Only Admin and Seller can access
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();

            if (store.UserId != userId && !User.IsInRole("admin"))
            {
                return Forbid();
            }

            await _storeService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search/{name}")]
        public async Task<IActionResult> Search(string name)
        {
            var stores = await _storeService.SearchStoresAsync(name);
            if (stores == null || !stores.Any())
            {
                return NotFound();
            }
            return Ok(stores);
        }
        
        [HttpGet("by-user-id/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreByUserId(string userId)
        {
            var stores = await _storeService.GetByUserId(userId);
            return Ok(stores);
        }
    }
}
