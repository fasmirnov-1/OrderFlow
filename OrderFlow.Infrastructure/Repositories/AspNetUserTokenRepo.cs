using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IAspNetUserTokenRepo и IDisposable
    public class AspNetUserTokenRepo : IAspNetUserTokenRepo
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public AspNetUserTokenRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateAsync(AspNetUserToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            // ПРОВЕРКА КОНСТРЕИНТА (Foreign Key): Проверяем, существует ли пользователь в таблице AspNetUsers
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == token.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints: Пользователь с ID '{token.UserId}' не существует в таблице AspNetUsers.");
            }

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ (Составной первичный ключ)
            bool tokenExists = await _context.AspNetUserTokens.AnyAsync(t =>
                t.UserId == token.UserId &&
                t.LoginProvider == token.LoginProvider &&
                t.Name == token.Name);

            if (tokenExists)
            {
                throw new ArgumentException($"Нарушение Primary Key Constraint: Токен для пользователя '{token.UserId}' под провайдером '{token.LoginProvider}' с именем '{token.Name}' уже существует.");
            }

            _context.AspNetUserTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<AspNetUserToken?> ReadAsync(string userId, string loginProvider, string name)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(loginProvider) || string.IsNullOrWhiteSpace(name))
                return null;

            // Поиск по составному первичному ключу согласно [PrimaryKey("UserId", "LoginProvider", "Name")]
            return await _context.AspNetUserTokens.FindAsync(userId, loginProvider, name);
        }

        public async Task<IEnumerable<AspNetUserToken>> GetAllByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return Enumerable.Empty<AspNetUserToken>();

            // Получение всех токенов конкретного пользователя без отслеживания изменений (AsNoTracking)
            return await _context.AspNetUserTokens
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(AspNetUserToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            // Проверяем внешний ключ пользователя при обновлении информации о токене
            bool userExists = await _context.AspNetUsers.AnyAsync(u => u.Id == token.UserId);
            if (!userExists)
            {
                throw new ArgumentException($"Нарушение Foreign Key Constraints при обновлении: Связанный пользователь с ID '{token.UserId}' не найден.");
            }

            _context.Entry(token).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId, string loginProvider, string name)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(loginProvider) || string.IsNullOrWhiteSpace(name))
                return;

            var token = await _context.AspNetUserTokens.FirstOrDefaultAsync(t =>
                t.UserId == userId &&
                t.LoginProvider == loginProvider &&
                t.Name == name);

            if (token != null)
            {
                _context.AspNetUserTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Ищет токен сессии по его значению и возвращает токен вместе с привязанным пользователем.
        /// </summary>
        public async Task<AspNetUserToken?> GetByTokenValueWithUserAsync(string tokenValue)
        {
            if (string.IsNullOrWhiteSpace(tokenValue))
                return null;

            return await _context.AspNetUserTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Value == tokenValue);
        }
    }
}