using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для выполнения CRUD операций над сущностью AspNetUserClaim
    /// с учетом ограничений ссылочной целостности базы данных.
    /// </summary>
    public interface IAspNetUserClaimRepo
    {
        /// <summary>
        /// Создает новый утверждение (клейм) для пользователя с предварительной валидацией внешнего ключа (UserId).
        /// </summary>
        Task<AspNetUserClaim> CreateAsync(AspNetUserClaim entity);

        /// <summary>
        /// Возвращает утверждение пользователя по его идентификатору. Если запись не найдена, возвращает null.
        /// </summary>
        Task<AspNetUserClaim?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает список всех утверждений пользователей без отслеживания изменений.
        /// </summary>
        Task<IEnumerable<AspNetUserClaim>> GetAllAsync();

        /// <summary>
        /// Обновляет существующее утверждение с обязательной проверкой существования UserId в базе данных.
        /// </summary>
        Task UpdateAsync(AspNetUserClaim entity);

        /// <summary>
        /// Удаляет утверждение по его идентификатору, если оно существует.
        /// </summary>
        Task DeleteAsync(int id);
    }
}