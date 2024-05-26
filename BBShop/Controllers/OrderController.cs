using BBShop.DTOs;
using BBShop.Models;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBShop.Controllers;

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

    [HttpPost]
    public async Task<IActionResult> Add(OrderCreateDto orderDto)
    {
        try
        {
            await _orderService.AddAsync(orderDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, OrderDto orderDto)
    {
        try
        {
            await _orderService.UpdateAsync(id, orderDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPut("status/{id}")]  
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatusUpdateDto statusUpdateDto)
    {
        try
        {
            await _orderService.UpdateStatusAsync(id, statusUpdateDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }
}
