using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBShop.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _context.Products.Include(p => p.Store).FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.Include(p => p.Store).ToListAsync();
    }
    
    public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId)  
    {
        return await _context.Products.Where(p => p.StoreId == storeId).ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}