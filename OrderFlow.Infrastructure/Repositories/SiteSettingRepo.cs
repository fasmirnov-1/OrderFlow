using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ISiteSettingRepo
    public class SiteSettingRepo : ISiteSettingRepo
    {
        private readonly AppDbContext _context;

        public SiteSettingRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SiteSetting> CreateAsync(SiteSetting siteSetting)
        {
            if (siteSetting == null) throw new ArgumentNullException(nameof(siteSetting));

            // Автоматическое заполнение дат при создании конфигурации
            var now = DateTime.UtcNow;
            siteSetting.CreatedAt = now;
            siteSetting.UpdatedAt = now;

            _context.SiteSettings.Add(siteSetting);
            await _context.SaveChangesAsync();
            return siteSetting;
        }

        public async Task<SiteSetting?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай отсутствия записи
            return await _context.SiteSettings.FindAsync(id);
        }

        public async Task<SiteSetting?> GetCurrentSettingsAsync()
        {
            // Оптимальный метод для быстрого получения текущих активных настроек (без трекинга)
            return await _context.SiteSettings
                .AsNoTracking()
                .OrderByDescending(s => s.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SiteSetting>> GetAllAsync()
        {
            // Применяем AsNoTracking() для оптимизации вывода истории изменений конфигураций (если применимо)
            return await _context.SiteSettings
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(SiteSetting siteSetting)
        {
            if (siteSetting == null) throw new ArgumentNullException(nameof(siteSetting));

            // Автоматически обновляем штамп времени изменения конфигурации
            siteSetting.UpdatedAt = DateTime.UtcNow;

            _context.Entry(siteSetting).State = EntityState.Modified;

            // Защищаем дату создания от случайного затирания или обнуления при обновлении графа
            _context.Entry(siteSetting).Property(x => x.CreatedAt).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Безопасное извлечение перед удалением
            var siteSetting = await _context.SiteSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (siteSetting != null)
            {
                _context.SiteSettings.Remove(siteSetting);
                await _context.SaveChangesAsync();
            }
        }
    }
}