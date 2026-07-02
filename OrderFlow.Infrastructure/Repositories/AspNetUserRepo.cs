using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IAspNetUserRepo
    public class AspNetUserRepo : IAspNetUserRepo
    {
        private readonly AppDbContext _context;

        public AspNetUserRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateAsync(AspNetUser user)
        {
            // Обязательно дожидаемся проверки уникальности
            await CheckUniqueConstraintsAsync(user, isUpdate: false);

            await _context.AspNetUsers.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AspNetUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return await _context.AspNetUsers.FindAsync(id);
        }

        public async Task<IEnumerable<AspNetUser>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации чтения списков
            return await _context.AspNetUsers
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(AspNetUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // ПРОВЕРКА КОНСТРЕИНТОВ УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущего пользователя)
            await CheckUniqueConstraintsAsync(user, isUpdate: true);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            // Подгружаем пользователя вместе с коллекцией ролей (Many-to-Many) для корректного разрыва связей
            var user = await _context.AspNetUsers
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                // 1. ПРОВЕРКА ССЫЛОЧНОЙ ЦЕЛОСТНОСТИ: Ограничение Restrict на аудит-логи (AdminLog)
                bool hasAdminLogs = await _context.AdminLogs.AnyAsync(al => al.AdminUserId == id);
                if (hasAdminLogs)
                {
                    throw new InvalidOperationException($"Нарушение Restrict Constraint: Нельзя удалить пользователя с ID '{id}', так как он зафиксирован в системных логах аудита (AdminLogs).");
                }

                // 2. Очистка связей Many-to-Many с ролями (разрываем связи в промежуточной таблице)
                user.Roles.Clear();

                // 3. Обработка связанных One-to-Many сущностей Identity (превентивное удаление клеймов, токенов и внешних логинов)
                var claims = _context.AspNetUserClaims.Where(c => c.UserId == id);
                _context.AspNetUserClaims.RemoveRange(claims);

                var logins = _context.AspNetUserLogins.Where(l => l.UserId == id);
                _context.AspNetUserLogins.RemoveRange(logins);

                var tokens = _context.AspNetUserTokens.Where(t => t.UserId == id);
                _context.AspNetUserTokens.RemoveRange(tokens);

                // 4. Удаление самого пользователя
                _context.AspNetUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Вспомогательный метод для превентивной валидации Unique Constraints полей UserName и Email.
        /// </summary>
        private async Task CheckUniqueConstraintsAsync(AspNetUser user, bool isUpdate = false)
        {
            // Вытягиваем флаги существования одним комбинированным запросом
            var existingUser = await _context.AspNetUsers
                .AsNoTracking()
                .Where(u => !isUpdate || u.Id != user.Id)
                .Select(u => new
                {
                    UserNameExists = u.UserName == user.UserName,
                    NormalizedNameExists = u.NormalizedUserName == user.NormalizedUserName,
                    EmailExists = u.Email == user.Email,
                    NormalizedEmailExists = u.NormalizedEmail == user.NormalizedEmail
                })
                .FirstOrDefaultAsync(u => u.UserNameExists || u.NormalizedNameExists || u.EmailExists || u.NormalizedEmailExists);

            if (existingUser != null)
            {
                if (existingUser.UserNameExists && !string.IsNullOrEmpty(user.UserName))
                    throw new ArgumentException($"Нарушение Unique Constraint: Имя пользователя '{user.UserName}' уже занято.");

                if (existingUser.NormalizedNameExists && !string.IsNullOrEmpty(user.NormalizedUserName))
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованное имя пользователя '{user.NormalizedUserName}' уже существует.");

                if (existingUser.EmailExists && !string.IsNullOrEmpty(user.Email))
                    throw new ArgumentException($"Нарушение Unique Constraint: Email '{user.Email}' уже зарегистрирован.");

                if (existingUser.NormalizedEmailExists && !string.IsNullOrEmpty(user.NormalizedEmail))
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованный Email '{user.NormalizedEmail}' уже зарегистрирован.");
            }
        }
        public async Task<bool> IsUserExists(string login)
        {
            // Исправлено: выбрасываем исключение, если строка ДЕЙСТВИТЕЛЬНО пустая или состоит из пробелов
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Логин не может быть пустым или состоять из пробелов.", nameof(login));
            }

            // Оптимизация: AnyAsync выполняет быстрый запрос в БД и возвращает true/false,
            // вообще не загружая сущности AspNetUser в оперативную память приложения.
            return await _context.AspNetUsers
                .AnyAsync(u => u.UserName == login);
        }
        public async Task<string?> GetPasswordByLoginAsync(string login)
        {
            // 1. Валидация входных данных
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Логин не может быть пустым или состоять из пробелов.", nameof(login));
            }

            // 2. Проекция через .Select() вытаскивает из БД исключительно строку хэша пароля.
            // Это генерирует оптимальный SQL-запрос вида: SELECT [u].[PasswordHash] FROM [AspNetUsers] AS [u] ...
            return await _context.AspNetUsers
                .Where(u => u.UserName == login)
                .Select(u => u.PasswordHash)
                .FirstOrDefaultAsync();
            // Если пользователь не найден, метод безопасно вернет null вместо падения.
        }
        public async Task<AspNetUser?> GetUserAsync(string login)
        {
            // Валидация входных данных, чтобы не делать холостой запрос в базу
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentException("Логин не может быть пустым или состоять из пробелов.", nameof(login));
            }

            return await _context.AspNetUsers
                .AsNoTracking() // Оптимизация производительности (не тратит ресурсы на Change Tracker)
                .Where(u => u.UserName == login)
                .SingleOrDefaultAsync(); // Гарантирует, что логин уникален. Возвращает null, если не найден.
        }
    }
}