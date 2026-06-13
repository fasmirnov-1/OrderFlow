using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс PortfolioCategoryRepo, осуществляющий crud операции над сущностью PortfolioCategory с учетом имеющихся констрент и ключей.
    public class PortfolioCategoryRepo
    {
        private AppDbContext _context;

        public PortfolioCategoryRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PortfolioCategory> CreateAsync(PortfolioCategory category)
        {
            _context.PortfolioCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<PortfolioCategory> GetByIdAsync(int id)
        {
            return await _context.PortfolioCategories.FindAsync(id);
        }

        public async Task<IEnumerable<PortfolioCategory>> GetAllAsync()
        {
            return await _context.PortfolioCategories.ToListAsync();
        }

        public async Task UpdateAsync(PortfolioCategory category)
        {
            _context.PortfolioCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.PortfolioCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
