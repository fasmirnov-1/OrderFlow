using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления пользователями (AspNetUsers) 
    /// с учетом ограничений уникальности Identity и ссылочной целостности базы данных.
    /// </summary>
    public interface IAspNetUserRepo
    {
        /// <summary>
        /// Создает нового пользователя с превентивной валидацией уникальности UserName и Email.
        /// </summary>
        Task<AspNetUser> CreateAsync(AspNetUser user);

        /// <summary>
        /// Возвращает пользователя по его строковому идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<AspNetUser?> GetByIdAsync(string id);

        /// <summary>
        /// Возвращает полный список всех зарегистрированных пользователей без отслеживания изменений.
        /// </summary>
        Task<IEnumerable<AspNetUser>> GetAllAsync();

        /// <summary>
        /// Обновляет данные существующего пользователя, проверяя ограничения уникальности среди других аккаунтов.
        /// </summary>
        Task UpdateAsync(AspNetUser user);

        /// <summary>
        /// Удаляет пользователя по его идентификатору, превентивно очищая зависимые сущности (Claims, Logins, Tokens, Roles) 
        /// и блокируя операцию, если нарушается Restrict Constraint (например, наличие записей в AdminLogs).
        /// </summary>
        Task DeleteAsync(string id);
    }
}