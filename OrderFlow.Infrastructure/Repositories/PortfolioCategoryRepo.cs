using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IPortfolioCategoryRepo
    public class PortfolioCategoryRepo : IPortfolioCategoryRepo
    {
        private readonly AppDbContext _context;

        public PortfolioCategoryRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PortfolioCategory> CreateAsync(PortfolioCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ: Проверяем Slug на дубликаты перед вставкой
            bool slugExists = await _context.PortfolioCategories.AnyAsync(c => c.Slug == category.Slug);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Категория портфолио со слагом '{category.Slug}' уже существует.");
            }

            _context.PortfolioCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<PortfolioCategory?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай отсутствия записи
            return await _context.PortfolioCategories.FindAsync(id);
        }

        public async Task<IEnumerable<PortfolioCategory>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списков в пользовательском интерфейсе и админке
            return await _context.PortfolioCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(PortfolioCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущую запись)
            bool slugExists = await _context.PortfolioCategories.AnyAsync(c => c.Slug == category.Slug && c.Id != category.Id);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: URL-слаг '{category.Slug}' уже используется другой категорией портфолио.");
            }

            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Находим категорию вместе со связанными элементами портфолио для контроля связей
            var category = await _context.PortfolioCategories
                .Include(c => c.PortfolioItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                // Защитное бизнес-правило: Если к категории привязаны проекты, 
                // сначала нужно убрать связи в проектах (или выдать ошибку, защищая целостность данных)
                if (category.PortfolioItems.Any())
                {
                    throw new InvalidOperationException($"Невозможно удалить категорию с ID {id}, так как к ней привязаны существующие проекты портфолио ({category.PortfolioItems.Count}).");
                }

                _context.PortfolioCategories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}