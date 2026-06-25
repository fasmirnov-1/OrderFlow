using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class AspNetRoleClaimRepo : IAspNetRoleClaimRepo
    {
        private readonly AppDbContext _context;

        public AspNetRoleClaimRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AspNetRoleClaim> CreateAsync(AspNetRoleClaim entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // ПРОВЕРКА КОНСТРЕИНТА (Foreign Key): Валидация связи RoleId -> AspNetRoles.Id
            bool roleExists = await _context.AspNetRoles.AnyAsync(r => r.Id == entity.RoleId);
            if (!roleExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints: Роль с ID '{entity.RoleId}' не зарегистрирована в AspNetRoles.");
            }

            _context.AspNetRoleClaims.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<AspNetRoleClaim?> GetByIdAsync(int id)
        {
            return await _context.AspNetRoleClaims.FindAsync(id);
        }

        public async Task<IEnumerable<AspNetRoleClaim>> GetAllAsync()
        {
            return await _context.AspNetRoleClaims.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(AspNetRoleClaim entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Проверка внешнего ключа роли перед обновлением
            bool roleExists = await _context.AspNetRoles.AnyAsync(r => r.Id == entity.RoleId);
            if (!roleExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints при обновлении: Назначаемая роль с ID '{entity.RoleId}' не найдена.");
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AspNetRoleClaims.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                _context.AspNetRoleClaims.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}