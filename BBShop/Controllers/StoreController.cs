using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBShop.Controllers;


[Route("api/[controller]")]
[ApiController]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var store = await _storeService.GetByIdAsync(id);
        if (store == null)
        {
            return NotFound();
        }
        return Ok(store);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stores = await _storeService.GetAllAsync();
        return Ok(stores);
    }

    [HttpPost]
    public async Task<IActionResult> Add(StoreCreateDto storeDto)
    {
        try
        {
            await _storeService.AddAsync(storeDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, StoreUpdateDto storeDto)
    {
        try
        {
            await _storeService.UpdateAsync(id, storeDto);
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
        await _storeService.DeleteAsync(id);
        return NoContent();
    }
}
