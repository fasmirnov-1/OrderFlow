using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать кдасс BlogCategoryRepo, который выполянет crud операции для сущности BlogCategory. с учетом текущих констрейнов и ключей.
    public class BlogCategoryRepo
    {
        private AppDbContext _context;

        public BlogCategoryRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlogCategory> CreateAsync(BlogCategory category)
        {
            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<BlogCategory> GetByIdAsync(int id)
        {
            return await _context.BlogCategories.FindAsync(id);
        }

        public async Task<IEnumerable<BlogCategory>> GetAllAsync()
        {
            return await _context.BlogCategories.ToListAsync();
        }

        public async Task UpdateAsync(BlogCategory category)
        {
            _context.BlogCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
            {
                _context.BlogCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
