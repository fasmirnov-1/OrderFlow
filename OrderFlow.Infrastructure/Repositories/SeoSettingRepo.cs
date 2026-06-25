using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ISeoSettingRepo
    public class SeoSettingRepo : ISeoSettingRepo
    {
        private readonly AppDbContext _context;

        public SeoSettingRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SeoSetting> CreateAsync(SeoSetting seoSetting)
        {
            if (seoSetting == null) throw new ArgumentNullException(nameof(seoSetting));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ: предотвращаем дублирование настроек для одного роута
            bool routeExists = await _context.SeoSettings
                .AnyAsync(s => s.PageRoute == seoSetting.PageRoute);

            if (routeExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: SEO-настройки для маршрута '{seoSetting.PageRoute}' уже существуют в базе данных.");
            }

            _context.SeoSettings.Add(seoSetting);
            await _context.SaveChangesAsync();
            return seoSetting;
        }

        public async Task<SeoSetting?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай отсутствия записи
            return await _context.SeoSettings.FindAsync(id);
        }

        public async Task<SeoSetting?> GetByRouteAsync(string pageRoute)
        {
            if (string.IsNullOrWhiteSpace(pageRoute)) return null;

            // Быстрая выборка без трекинга по уникальному индексу IX_SeoSettings_PageRoute.
            // Используется на фронтенде при загрузке страниц для рендеринга мета-тегов.
            return await _context.SeoSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.PageRoute == pageRoute);
        }

        public async Task<IEnumerable<SeoSetting>> GetAllAsync()
        {
            // Применяем AsNoTracking() для оптимизации вывода списка всех страниц в админ-панели
            return await _context.SeoSettings
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(SeoSetting seoSetting)
        {
            if (seoSetting == null) throw new ArgumentNullException(nameof(seoSetting));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущую запись по Id)
            bool routeExists = await _context.SeoSettings
                .AnyAsync(s => s.PageRoute == seoSetting.PageRoute && s.Id != seoSetting.Id);

            if (routeExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Невозможно обновить запись. Маршрут '{seoSetting.PageRoute}' уже закреплен за другими SEO-настройками.");
            }

            _context.Entry(seoSetting).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Используем FirstOrDefaultAsync для безопасного извлечения сущности перед удалением
            var seoSetting = await _context.SeoSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (seoSetting != null)
            {
                _context.SeoSettings.Remove(seoSetting);
                await _context.SaveChangesAsync();
            }
        }
    }
}