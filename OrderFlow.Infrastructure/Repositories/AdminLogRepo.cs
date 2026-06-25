using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class AdminLogRepo : IAdminLogRepo
    {
        private readonly AppDbContext _context;

        public AdminLogRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AdminLog> CreateAsync(AdminLog adminLog)
        {
            if (adminLog == null) throw new ArgumentNullException(nameof(adminLog));

            // ПРОВЕРКА КОНСТРЕИНТА (Foreign Key): Валидация AdminUserId по таблице AspNetUsers
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == adminLog.AdminUserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key: Администратор с ID '{adminLog.AdminUserId}' не найден в таблице AspNetUsers.");
            }

            _context.AdminLogs.Add(adminLog);
            await _context.SaveChangesAsync();
            return adminLog;
        }

        public async Task<AdminLog?> GetByIdAsync(int id)
        {
            return await _context.AdminLogs.FindAsync(id);
        }

        public async Task<IEnumerable<AdminLog>> GetAllAsync()
        {
            return await _context.AdminLogs.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(AdminLog adminLog)
        {
            if (adminLog == null) throw new ArgumentNullException(nameof(adminLog));

            // Проверка внешнего ключа при редактировании записи лога
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == adminLog.AdminUserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key при обновлении: Администратор с ID '{adminLog.AdminUserId}' не существует.");
            }

            _context.Entry(adminLog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var adminLog = await _context.AdminLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (adminLog != null)
            {
                _context.AdminLogs.Remove(adminLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}