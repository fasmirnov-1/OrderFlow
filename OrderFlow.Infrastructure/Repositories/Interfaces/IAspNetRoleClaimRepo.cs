using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    public interface IAspNetRoleClaimRepo
    {
        /// <summary>
        /// Создает новый клейм для роли с предварительной проверкой существования роли (Foreign Key).
        /// </summary>
        Task<AspNetRoleClaim> CreateAsync(AspNetRoleClaim entity);

        /// <summary>
        /// Возвращает клейм по его идентификатору. Если запись не найдена, возвращает null.
        /// </summary>
        Task<AspNetRoleClaim?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает список всех клеймов ролей без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<AspNetRoleClaim>> GetAllAsync();

        /// <summary>
        /// Обновляет существующий клейм с валидацией внешнего ключа роли.
        /// </summary>
        Task UpdateAsync(AspNetRoleClaim entity);

        /// <summary>
        /// Удаляет клейм по его идентификатору, если он существует.
        /// </summary>
        Task DeleteAsync(int id);
    }
}