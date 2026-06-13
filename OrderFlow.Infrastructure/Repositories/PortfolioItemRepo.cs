using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс PortfolioItemRepo, осуществляющий crud операции над сущностью PortfolioItem с учетом имеющихся констрент и ключей.
    public class PortfolioItemRepo
    {
        private AppDbContext _context;
        public PortfolioItemRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PortfolioItem> CreateAsync(PortfolioItem item)
        {
            _context.PortfolioItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }
        public async Task<PortfolioItem> GetByIdAsync(Guid id)
        {
            return await _context.PortfolioItems.FindAsync(id);
        }
        public async Task<IEnumerable<PortfolioItem>> GetAllAsync()
        {
            var portfolios = await _context.PortfolioItems.ToListAsync();
            return portfolios;
        }
        public async Task UpdateAsync(PortfolioItem item)
        {
            _context.PortfolioItems.Update(item);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.PortfolioItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
