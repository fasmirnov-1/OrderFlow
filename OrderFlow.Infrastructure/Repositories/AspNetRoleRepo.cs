using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AspNetRoleRepo, который будет реализовывать интерфейс IAspNetRoleRepo и использовать контекст базы данных для выполнения операций CRUD над сущностью AspNetRole с учетом имеющихся констрент и ключей.
    public class AspNetRoleRepo
    {
        private AppDbContext _context;
        public AspNetRoleRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AspNetRole> GetByIdAsync(string id)
        {
            return await _context.AspNetRoles.FindAsync(id);
        }
        public async Task<IEnumerable<AspNetRole>> GetAllAsync()
        {
            return await _context.AspNetRoles.ToListAsync();
        }
        public async Task AddAsync(AspNetRole role)
        {
            _context.AspNetRoles.Add(role);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(AspNetRole role)
        {
            _context.AspNetRoles.Update(role);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(string id)
        {
            var role = await GetByIdAsync(id);
            if (role != null)
            {
                _context.AspNetRoles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}
