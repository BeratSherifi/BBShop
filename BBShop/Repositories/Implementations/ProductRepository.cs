using BBShop.Data;
using BBShop.Models;
using BBShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BBShop.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Store).Include(p => p.User).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Store).Include(p => p.User).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByStoreNameAsync(string storeName)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Where(p => p.Store.StoreName == storeName)
                .ToListAsync();
        }
        
     public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Where(p => p.StoreId == storeId)
                .ToListAsync();
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

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}