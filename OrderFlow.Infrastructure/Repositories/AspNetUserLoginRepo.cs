using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IAspNetUserLoginRepo
    public class AspNetUserLoginRepo : IAspNetUserLoginRepo
    {
        private readonly AppDbContext _context;

        public AspNetUserLoginRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateAsync(AspNetUserLogin entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // ПРОВЕРКА КОНСТРЕИНТА (Foreign Key): Проверяем, существует ли пользователь в таблице AspNetUsers
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == entity.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints: Пользователь с ID '{entity.UserId}' не существует в таблице AspNetUsers.");
            }

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ (Первичный ключ): Проверяем, нет ли уже провайдера с таким ключом
            bool loginExists = await _context.AspNetUserLogins.AnyAsync(l =>
                l.LoginProvider == entity.LoginProvider &&
                l.ProviderKey == entity.ProviderKey);
            if (loginExists)
            {
                throw new ArgumentException($"Нарушение Primary Key/Unique Constraint: Запись внешнего входа для провайдера '{entity.LoginProvider}' с ключом '{entity.ProviderKey}' уже зарегистрирована.");
            }

            _context.AspNetUserLogins.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<AspNetUserLogin?> ReadAsync(string loginProvider, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(loginProvider) || string.IsNullOrWhiteSpace(providerKey)) return null;

            // Передаем параметры строго в порядке объявления [PrimaryKey("LoginProvider", "ProviderKey")]
            return await _context.AspNetUserLogins.FindAsync(loginProvider, providerKey);
        }

        public async Task<IEnumerable<AspNetUserLogin>> GetAllByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return Enumerable.Empty<AspNetUserLogin>();

            // Метод для получения всех внешних входов конкретного пользователя (AsNoTracking для оптимизации чтения)
            return await _context.AspNetUserLogins
                .AsNoTracking()
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(AspNetUserLogin entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Проверяем внешний ключ пользователя при обновлении информации о входе
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == entity.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints при обновлении: Связанный пользователь с ID '{entity.UserId}' не найден.");
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string loginProvider, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(loginProvider) || string.IsNullOrWhiteSpace(providerKey)) return;

            // Поиск по точным ключам сущности
            var entity = await _context.AspNetUserLogins.FirstOrDefaultAsync(l =>
                l.LoginProvider == loginProvider &&
                l.ProviderKey == providerKey);

            if (entity != null)
            {
                _context.AspNetUserLogins.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}