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

        public async Task<AspNetUser> CreateAsync(AspNetUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // ПРОВЕРКА КОНСТРЕИНТОВ УНИКАЛЬНОСТИ (Превентивная валидация)
            await CheckUniqueConstraintsAsync(user);

            _context.AspNetUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
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
            if (!string.IsNullOrEmpty(user.UserName))
            {
                bool userNameExists = isUpdate
                    ? await _context.AspNetUsers.AnyAsync(u => u.UserName == user.UserName && u.Id != user.Id)
                    : await _context.AspNetUsers.AnyAsync(u => u.UserName == user.UserName);

                if (userNameExists)
                    throw new ArgumentException($"Нарушение Unique Constraint: Имя пользователя '{user.UserName}' уже занято.");
            }

            if (!string.IsNullOrEmpty(user.NormalizedUserName))
            {
                bool normalizedNameExists = isUpdate
                    ? await _context.AspNetUsers.AnyAsync(u => u.NormalizedUserName == user.NormalizedUserName && u.Id != user.Id)
                    : await _context.AspNetUsers.AnyAsync(u => u.NormalizedUserName == user.NormalizedUserName);

                if (normalizedNameExists)
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованное имя пользователя '{user.NormalizedUserName}' уже существует.");
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                bool emailExists = isUpdate
                    ? await _context.AspNetUsers.AnyAsync(u => u.Email == user.Email && u.Id != user.Id)
                    : await _context.AspNetUsers.AnyAsync(u => u.Email == user.Email);

                if (emailExists)
                    throw new ArgumentException($"Нарушение Unique Constraint: Email '{user.Email}' уже зарегистрирован.");
            }

            if (!string.IsNullOrEmpty(user.NormalizedEmail))
            {
                bool normalizedEmailExists = isUpdate
                    ? await _context.AspNetUsers.AnyAsync(u => u.NormalizedEmail == user.NormalizedEmail && u.Id != user.Id)
                    : await _context.AspNetUsers.AnyAsync(u => u.NormalizedEmail == user.NormalizedEmail);

                if (normalizedEmailExists)
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованный Email '{user.NormalizedEmail}' уже зарегистрирован.");
            }
        }
    }
}