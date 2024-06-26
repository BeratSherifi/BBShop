using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BBShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("by-store/{storeId}")]
        [Authorize(Policy = "SellerPolicy")]
        public async Task<IActionResult> GetByStoreId(Guid storeId) 
        {
            var orders = await _orderService.GetByStoreIdAsync(storeId);
            return Ok(orders);
        }

        [HttpGet("by-user/{userId}")]
        [Authorize(Policy = "BuyerPolicy")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var orders = await _orderService.GetByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost]
        [Authorize(Policy = "BuyerPolicy")]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto orderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.AddAsync(orderDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "SellerPolicy")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            await _orderService.UpdateStatusAsync(id, status);
            return NoContent();
        }
    }
}