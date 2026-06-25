using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ISystemNotificationRepo
    public class SystemNotificationRepo : ISystemNotificationRepo
    {
        private readonly AppDbContext _context;

        public SystemNotificationRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SystemNotification> CreateAsync(SystemNotification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            // Автоматическая установка даты создания уведомления, если она не задана
            if (notification.CreatedAt == default)
            {
                notification.CreatedAt = DateTime.UtcNow;
            }

            _context.SystemNotifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<SystemNotification?> GetByIdAsync(int id)
        {
            // Исправлен тип ключа на int, возвращается nullable-тип
            return await _context.SystemNotifications.FindAsync(id);
        }

        public async Task<IEnumerable<SystemNotification>> GetAllAsync()
        {
            // Применяем AsNoTracking() для оптимизации вывода списков.
            // Сортируем по индексу CreatedAt: новые уведомления должны быть первыми в списке.
            return await _context.SystemNotifications
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SystemNotification>> GetUnreadAdminNotificationsAsync()
        {
            // Оптимизированный метод выборки непрочитанных логов/уведомлений для верхнего колокола (админка)
            // Использует индексы IX_SystemNotifications_IsRead и IX_SystemNotifications_CreatedAt
            return await _context.SystemNotifications
                .AsNoTracking()
                .Where(n => !n.IsRead && n.ForAdminOnly == true)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(SystemNotification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            _context.Entry(notification).State = EntityState.Modified;

            // Предотвращаем изменение даты создания при обновлении статуса (например, при пометке как "Прочитано")
            _context.Entry(notification).Property(x => x.CreatedAt).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Используем FirstOrDefaultAsync для быстрой и безопасной очистки трекера контекста
            var notification = await _context.SystemNotifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification != null)
            {
                _context.SystemNotifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}