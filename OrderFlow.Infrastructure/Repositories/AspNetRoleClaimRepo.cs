using Microsoft.EntityFrameworkCore;

using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AspNetRoleClaimRepo, который будет реализовывать интерфейс IAspNetRoleClaimRepo и выполнять операции CRUD для сущности AspNetRoleClaim с учетом имеющихся констрент и ключей.
    public class AspNetRoleClaimRepo
    {
        private AppDbContext _context;
        public AspNetRoleClaimRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AspNetRoleClaim> CreateAsync(AspNetRoleClaim entity)
        {
            _context.AspNetRoleClaims.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<AspNetRoleClaim> GetByIdAsync(int id)
        {
            return await _context.AspNetRoleClaims.FindAsync(id);
        }
        public async Task<IEnumerable<AspNetRoleClaim>> GetAllAsync()
        {
            return await _context.AspNetRoleClaims.ToListAsync();
        }
        public async Task UpdateAsync(AspNetRoleClaim entity)
        {
            _context.AspNetRoleClaims.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AspNetRoleClaims.FindAsync(id);
            if (entity != null)
            {
                _context.AspNetRoleClaims.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
