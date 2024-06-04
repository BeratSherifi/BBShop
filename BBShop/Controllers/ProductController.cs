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

        [HttpGet("store/{storeName}")]
        public async Task<IActionResult> GetByStoreName(string storeName)
        {
            var products = await _productService.GetByStoreNameAsync(storeName);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrSellerPolicy")]
        public async Task<IActionResult> Add([FromForm] ProductCreateDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _productService.AddAsync(productDto, userId);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrSellerPolicy")]
        public async Task<IActionResult> Update(Guid id, [FromForm] ProductUpdateDto productDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _productService.UpdateAsync(id, productDto, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrSellerPolicy")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _productService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
