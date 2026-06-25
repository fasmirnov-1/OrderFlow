using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    public class AspNetRoleRepo : IAspNetRoleRepo
    {
        private readonly AppDbContext _context;

        public AspNetRoleRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AspNetRole?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return await _context.AspNetRoles.FindAsync(id);
        }

        public async Task<IEnumerable<AspNetRole>> GetAllAsync()
        {
            return await _context.AspNetRoles.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(AspNetRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            // ПРОВЕРКА КОНСТРЕИНТОВ УНИКАЛЬНОСТИ ИМЕН (Индексы Unique)
            if (!string.IsNullOrEmpty(role.Name))
            {
                bool nameExists = await _context.AspNetRoles.AnyAsync(r => r.Name == role.Name);
                if (nameExists)
                {
                    throw new ArgumentException($"Нарушение Unique Constraint: Роль '{role.Name}' уже существует.");
                }
            }

            if (!string.IsNullOrEmpty(role.NormalizedName))
            {
                bool normalizedExists = await _context.AspNetRoles.AnyAsync(r => r.NormalizedName == role.NormalizedName);
                if (normalizedExists)
                {
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованное наименование '{role.NormalizedName}' уже занято.");
                }
            }

            _context.AspNetRoles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AspNetRole role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            // ПРОВЕРКА УНИКАЛЬНОСТИ ПРИ ИЗМЕНЕНИИ ТЕКУЩЕЙ РОЛИ
            if (!string.IsNullOrEmpty(role.Name))
            {
                bool nameExists = await _context.AspNetRoles.AnyAsync(r => r.Name == role.Name && r.Id != role.Id);
                if (nameExists)
                {
                    throw new ArgumentException($"Нарушение Unique Constraint: Имя роли '{role.Name}' уже используется другим объектом.");
                }
            }

            if (!string.IsNullOrEmpty(role.NormalizedName))
            {
                bool normalizedExists = await _context.AspNetRoles.AnyAsync(r => r.NormalizedName == role.NormalizedName && r.Id != role.Id);
                if (normalizedExists)
                {
                    throw new ArgumentException($"Нарушение Unique Constraint: Нормализованное имя '{role.NormalizedName}' совпадает с существующей записью.");
                }
            }

            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var role = await _context.AspNetRoles.FirstOrDefaultAsync(r => r.Id == id);
            if (role != null)
            {
                // ПРОВЕРКА ССЫЛОЧНОЙ ЦЕЛОСТНОСТИ (Foreign Key / Restrict) ПЕРЕД УДАЛЕНИЕМ РОЛИ:

                // 1. Проверяем связанные RoleClaims (Связь One-to-Many со стороны роли)
                bool hasClaims = await _context.AspNetRoleClaims.AnyAsync(rc => rc.RoleId == id);
                if (hasClaims)
                {
                    throw new InvalidOperationException($"Нарушение Restrict Constraint: Удаление запрещено. К роли привязаны операционные права (AspNetRoleClaims).");
                }

                // 2. Проверяем связь Many-to-Many через навигационную коллекцию пользователей
                bool hasUsers = await _context.AspNetUsers.AnyAsync(u => u.Roles.Any(r => r.Id == id));
                if (hasUsers)
                {
                    throw new InvalidOperationException($"Нарушение Restrict Constraint: Удаление отклонено. Роль назначена активным пользователям (AspNetUsers).");
                }

                _context.AspNetRoles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }
}