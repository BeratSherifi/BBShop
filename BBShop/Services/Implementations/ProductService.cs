using AutoMapper;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;

namespace BBShop.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
    
    public async Task<IEnumerable<ProductDto>> GetByStoreIdAsync(Guid storeId)  
    {
        var products = await _productRepository.GetByStoreIdAsync(storeId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task AddAsync(ProductCreateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        await _productRepository.AddAsync(product);
    }

    public async Task UpdateAsync(Guid productId, ProductUpdateDto productDto)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new Exception("Product not found");
        }

        _mapper.Map(productDto, product);
        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }
}