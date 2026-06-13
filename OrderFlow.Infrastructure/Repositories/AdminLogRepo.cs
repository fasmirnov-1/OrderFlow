using Microsoft.EntityFrameworkCore;

using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AdminLogRepo, который будет реализовывать интерфейс IAdminLogRepo и использовать контекст базы данных для выполнения операций CRUD над сущностью AdminLog, при выполнении операций учитывать существующие констренты.
    public class AdminLogRepo
    {
        private AppDbContext _context;
        public AdminLogRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AdminLog> CreateAsync(AdminLog adminLog)
        {
            _context.AdminLogs.Add(adminLog);
            await _context.SaveChangesAsync();
            return adminLog;
        }
        public async Task<AdminLog> GetByIdAsync(int id)
        {
            return await _context.AdminLogs.FindAsync(id);
        }
        public async Task<IEnumerable<AdminLog>> GetAllAsync()
        {
            return await _context.AdminLogs.ToListAsync();
        }
        public async Task UpdateAsync(AdminLog adminLog)
        {
            _context.Entry(adminLog).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var adminLog = await _context.AdminLogs.FindAsync(id);
            if (adminLog != null)
            {
                _context.AdminLogs.Remove(adminLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
