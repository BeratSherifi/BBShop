using BBShop.DTOs;
using BBShop.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BBShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IMapper _mapper;

        public StoreController(IStoreService storeService, IMapper mapper)
        {
            _storeService = storeService;
            _mapper = mapper;
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
        [Authorize(Policy = "AdminOrSellerPolicy")] // Only Admin and Seller can access
        public async Task<IActionResult> Create([FromBody] StoreCreateDto storeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdStore = await _storeService.AddAsync(storeDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdStore.StoreId }, createdStore);
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSellerPolicy")] // Only Admin and Seller can access
        public async Task<IActionResult> Update(Guid id, [FromBody] StoreUpdateDto storeDto)
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
        [AllowAnonymous]
        public async Task<IActionResult> Search(string name)
        {
            var stores = await _storeService.SearchByNameAsync(name);
            return Ok(stores);
        }
    }
}