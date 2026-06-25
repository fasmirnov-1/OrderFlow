using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IPortfolioItemRepo
    public class PortfolioItemRepo : IPortfolioItemRepo
    {
        private readonly AppDbContext _context;

        public PortfolioItemRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PortfolioItem> CreateAsync(PortfolioItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Автоматическая установка даты создания, если она не была инициализирована
            if (item.CreatedAt == default)
            {
                item.CreatedAt = DateTime.UtcNow;
            }

            _context.PortfolioItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PortfolioItem?> GetByIdAsync(int id)
        {
            // Исправлен тип на int. Подгружаем связанные категории (Eager Loading)
            return await _context.PortfolioItems
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PortfolioItem>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации тяжелых выборок для UI / Админки
            // Сортируем по индексу CreatedAt (сначала новые проекты)
            return await _context.PortfolioItems
                .Include(p => p.Categories)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PortfolioItem>> GetPublishedAsync()
        {
            // Быстрая выборка благодаря композиции индексов IsPublished и CreatedAt
            return await _context.PortfolioItems
                .AsNoTracking()
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(PortfolioItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // Явное переключение стейта для обновления скалярных свойств. 
            // Примечание: Если вы обновляете коллекции Categories внутри item, 
            // это потребует дополнительной обработки связей через context.Entry() или работу с трекером.
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Ищем запись, чтобы EF Core мог корректно удалить связанные записи 
            // в связующей таблице Many-to-Many (PortfolioItemPortfolioCategory)
            var item = await _context.PortfolioItems
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (item != null)
            {
                // Очищаем связи перед удалением самого объекта (для безопасности работы трекера изменений)
                item.Categories.Clear();

                _context.PortfolioItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}