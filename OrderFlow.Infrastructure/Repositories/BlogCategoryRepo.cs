using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IBlogCategoryRepo
    public class BlogCategoryRepo : IBlogCategoryRepo
    {
        private readonly AppDbContext _context;

        public BlogCategoryRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BlogCategory> CreateAsync(BlogCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ: Проверяем Slug на дубликаты перед вставкой
            bool slugExists = await _context.BlogCategories.AnyAsync(c => c.Slug == category.Slug);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Категория со слагом '{category.Slug}' уже существует.");
            }

            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<BlogCategory?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай, если категория не найдена
            return await _context.BlogCategories.FindAsync(id);
        }

        public async Task<IEnumerable<BlogCategory>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации запросов на чтение списков
            return await _context.BlogCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(BlogCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущую запись)
            bool slugExists = await _context.BlogCategories.AnyAsync(c => c.Slug == category.Slug && c.Id != category.Id);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Слаг '{category.Slug}' уже используется другой категорией.");
            }

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.BlogCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                // ПРОВЕРКА ССЫЛОЧНОЙ ЦЕЛОСТНОСТИ (Restrict Constraint): 
                // Запрещаем удаление, если к категории привязаны статьи (BlogPosts)
                bool hasPosts = await _context.BlogPosts.AnyAsync(p => p.CategoryId == id);
                if (hasPosts)
                {
                    throw new InvalidOperationException($"Нарушение Restrict Constraint: Нельзя удалить категорию с ID {id}, так как в ней содержатся опубликованные или черновые статьи (BlogPosts).");
                }

                _context.BlogCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}