using AutoMapper;
using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Repositories.Implementations
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StoreRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Store> GetByIdAsync(Guid id)
        {
            return await _context.Stores
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StoreId == id);
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
                .Include(s => s.User)
                .Where(s => s.StoreName.Contains(name))
                .ToListAsync();
        }

        public async Task<Store> GetByUserIdAsync(string userId)
        {
            return await _context.Stores
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}