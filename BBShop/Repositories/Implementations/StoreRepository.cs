using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBShop.Repositories.Implementations;

public class StoreRepository : IStoreRepository
{
    private readonly AppDbContext _context;

    public StoreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Store> GetByIdAsync(Guid id)
    {
        return await _context.Stores.Include(s => s.User).FirstOrDefaultAsync(s => s.StoreId == id);
    }

    public async Task<IEnumerable<Store>> GetAllAsync()
    {
        return await _context.Stores.Include(s => s.User).ToListAsync();
    }

    public async Task AddAsync(Store store)
    {
        await _context.Stores.AddAsync(store);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Store store)
    {
        _context.Stores.Update(store);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
        }
    }
}