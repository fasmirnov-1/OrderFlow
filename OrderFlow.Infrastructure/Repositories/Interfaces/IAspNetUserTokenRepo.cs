using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления токенами безопасности пользователей (AspNetUserTokens)
    /// с контролем ограничений составных ключей и ссылочной целостности.
    /// </summary>
    public interface IAspNetUserTokenRepo
    {
        /// <summary>
        /// Добавляет новый токен безопасности с валидацией существования пользователя и отсутствия дубликатов составного ключа.
        /// </summary>
        Task CreateAsync(AspNetUserToken token);

        /// <summary>
        /// Ищет токен по составному первичному ключу (UserId + LoginProvider + Name).
        /// </summary>
        Task<AspNetUserToken?> ReadAsync(string userId, string loginProvider, string name);

        /// <summary>
        /// Возвращает все активные токены безопасности для конкретного пользователя.
        /// </summary>
        Task<IEnumerable<AspNetUserToken>> GetAllByUserIdAsync(string userId);

        /// <summary>
        /// Обновляет значение или параметры существующего токена.
        /// </summary>
        Task UpdateAsync(AspNetUserToken token);

        /// <summary>
        /// Удаляет токен безопасности по его составному первичному ключу.
        /// </summary>
        Task DeleteAsync(string userId, string loginProvider, string name);
    }
}