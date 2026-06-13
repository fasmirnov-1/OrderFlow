using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс SystemNotificationRepo, осуществляющий crud операции над сущностью SystemNotification с учетом имеющихся констрент и ключей.
    public class SystemNotificationRepo
    {
        private AppDbContext _context;
        public SystemNotificationRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<SystemNotification> CreateAsync(SystemNotification notification)
        {
            _context.SystemNotifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }
        public async Task<SystemNotification> GetByIdAsync(Guid id)
        {
            return await _context.SystemNotifications.FindAsync(id);
        }
        public async Task<IEnumerable<SystemNotification>> GetAllAsync()
        {
            return await _context.SystemNotifications.ToListAsync();
        }
        public async Task UpdateAsync(SystemNotification notification)
        {
            _context.SystemNotifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var notification = await GetByIdAsync(id);
            if (notification != null)
            {
                _context.SystemNotifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
