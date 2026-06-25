using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ITechnologyRepo
    public class TechnologyRepo : ITechnologyRepo
    {
        private readonly AppDbContext _context;

        public TechnologyRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Technology> CreateAsync(Technology technology)
        {
            if (technology == null) throw new ArgumentNullException(nameof(technology));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ (Бизнес-правило): предотвращаем дублирование полей, 
            // если в модели настроен уникальный индекс по Name или Slug
            // bool exists = await _context.Technologies.AnyAsync(t => t.Name == technology.Name);
            // if (exists) throw new ArgumentException($"Технология '{technology.Name}' уже существует.");

            _context.Technologies.Add(technology);
            await _context.SaveChangesAsync();
            return technology;
        }

        public async Task<Technology?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай отсутствия записи, подгружая связанные проекты
            return await _context.Technologies
                .Include(t => t.PortfolioItems)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Technology>> GetAllAsync()
        {
            // Применяем AsNoTracking() для тяжелых выборок в админке или фильтрах фронтенда
            return await _context.Technologies
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(Technology technology)
        {
            if (technology == null) throw new ArgumentNullException(nameof(technology));

            _context.Entry(technology).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Извлекаем технологию вместе с коллекцией связей Many-to-Many
            var technology = await _context.Technologies
                .Include(t => t.PortfolioItems)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technology != null)
            {
                // Явно очищаем коллекцию навигационного свойства перед удалением
                // Это заставит EF Core сгенерировать корректные DELETE-запросы для промежуточной таблицы связей
                technology.PortfolioItems.Clear();

                _context.Technologies.Remove(technology);
                await _context.SaveChangesAsync();
            }
        }
    }
}