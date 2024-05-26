using BBShop.DTOs;
using BBShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet("store/{storeId}")]  
    public async Task<IActionResult> GetByStoreId(Guid storeId)
    {
        var products = await _productService.GetByStoreIdAsync(storeId);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Add(ProductCreateDto productDto)
    {
        await _productService.AddAsync(productDto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ProductUpdateDto productDto)
    {
        try
        {
            await _productService.UpdateAsync(id, productDto);
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
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
