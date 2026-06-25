using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IAspNetUserClaimRepo
    public class AspNetUserClaimRepo : IAspNetUserClaimRepo
    {
        private readonly AppDbContext _context;

        public AspNetUserClaimRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AspNetUserClaim> CreateAsync(AspNetUserClaim entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // ПРОВЕРКА КОНСТРЕИНТА (Foreign Key): Проверяем существование UserId в таблице AspNetUsers перед добавлением клейма
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == entity.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints: Пользователь с ID '{entity.UserId}' не существует в таблице AspNetUsers.");
            }

            _context.AspNetUserClaims.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<AspNetUserClaim?> GetByIdAsync(int id)
        {
            // Возвращаем nullable тип, так как запись может отсутствовать
            return await _context.AspNetUserClaims.FindAsync(id);
        }

        public async Task<IEnumerable<AspNetUserClaim>> GetAllAsync()
        {
            // Используем AsNoTracking() для легковесного чтения без нагрузки на Change Tracker
            return await _context.AspNetUserClaims
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(AspNetUserClaim entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Проверяем внешний ключ пользователя при обновлении (защита на случай подмены UserId)
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == entity.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints при обновлении: Назначаемый пользователь с ID '{entity.UserId}' не найден.");
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AspNetUserClaims.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                _context.AspNetUserClaims.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}