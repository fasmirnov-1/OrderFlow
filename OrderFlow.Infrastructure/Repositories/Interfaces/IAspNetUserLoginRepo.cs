using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с внешними провайдерами авторизации пользователей (AspNetUserLogins)
    /// с превентивным контролем ограничений составных ключей и ссылочной целостности.
    /// </summary>
    public interface IAspNetUserLoginRepo
    {
        /// <summary>
        /// Добавляет новый внешний вход для пользователя с валидацией существования аккаунта и отсутствия дубликатов ключей провайдера.
        /// </summary>
        Task CreateAsync(AspNetUserLogin entity);

        /// <summary>
        /// Ищет запись внешнего входа по составному первичному ключу (LoginProvider + ProviderKey).
        /// </summary>
        Task<AspNetUserLogin?> ReadAsync(string loginProvider, string providerKey);

        /// <summary>
        /// Возвращает все зарегистрированные внешние входы для конкретного пользователя.
        /// </summary>
        Task<IEnumerable<AspNetUserLogin>> GetAllByUserIdAsync(string userId);

        /// <summary>
        /// Обновляет метаданные существующего внешнего входа (например, ProviderDisplayName).
        /// </summary>
        Task UpdateAsync(AspNetUserLogin entity);

        /// <summary>
        /// Удаляет привязку внешнего входа по составному первичному ключу.
        /// </summary>
        Task DeleteAsync(string loginProvider, string providerKey);
    }
}