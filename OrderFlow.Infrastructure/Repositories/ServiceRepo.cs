using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IServiceRepo
    public class ServiceRepo : IServiceRepo
    {
        private readonly AppDbContext _context;

        public ServiceRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Service> CreateAsync(Service service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай, если запись отсутствует
            return await _context.Services.FindAsync(id);
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списка в панели администратора.
            // Сортируем по DisplayOrder для сохранения правильной последовательности на UI.
            return await _context.Services
                .AsNoTracking()
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveAsync()
        {
            // Специализированный метод для фронтенда — извлекает только активные услуги
            return await _context.Services
                .AsNoTracking()
                .Where(s => s.IsActive == true)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        public async Task UpdateAsync(Service service)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            _context.Entry(service).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Использование FirstOrDefaultAsync для безопасной выборки объекта перед удалением
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}