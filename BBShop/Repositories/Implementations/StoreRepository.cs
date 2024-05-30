using AutoMapper;
using BBShop.Data;
using BBShop.DTOs;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using BBShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace BBShop.Repositories.Implementations
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Store> GetByIdAsync(Guid id)
        {
            return await _context.Stores.FindAsync(id);
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

        public async Task DeleteAsync(Store store)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Store>> SearchByNameAsync(string name)
        {
            return await _context.Stores
                .Where(s => s.StoreName.Contains(name))
                .ToListAsync();
        }
        
        public async Task<Store> GetByUserIdAsync(string userId)
        {
            return await _context.Stores.FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}