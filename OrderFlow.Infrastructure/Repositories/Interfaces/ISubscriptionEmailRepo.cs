using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления подписками на Email-рассылку (SubscriptionEmails)
    /// с контролем уникальности адресов и токенов отписки.
    /// </summary>
    public interface ISubscriptionEmailRepo
    {
        /// <summary>
        /// Создает новую подписку с превентивной валидацией уникальности Email и генерацией UnsubscribeToken.
        /// </summary>
        Task<SubscriptionEmail> CreateAsync(SubscriptionEmail subscriptionEmail);

        /// <summary>
        /// Возвращает запись подписки по ее уникальному идентификатору (Id). Если не найдена — возвращает null.
        /// </summary>
        Task<SubscriptionEmail?> GetByIdAsync(int id);

        /// <summary>
        /// Находит подписку по адресу электронной почты (использует индекс).
        /// </summary>
        Task<SubscriptionEmail?> GetByEmailAsync(string email);

        /// <summary>
        /// Находит подписку по уникальному токену отписки (используется для прекращения подписки из тела письма).
        /// </summary>
        Task<SubscriptionEmail?> GetByTokenAsync(string token);

        /// <summary>
        /// Возвращает полный список всех email в базе данных (активных и неактивных) без отслеживания изменений.
        /// </summary>
        Task<IEnumerable<SubscriptionEmail>> GetAllAsync();

        /// <summary>
        /// Возвращает список только активных подписчиков для непосредственной массовой отправки писем.
        /// </summary>
        Task<IEnumerable<SubscriptionEmail>> GetActiveSubscriptionsAsync();

        /// <summary>
        /// Обновляет параметры подписки (например, изменение флага IsActive или имени), проверяя ограничения на дубликаты.
        /// </summary>
        Task UpdateAsync(SubscriptionEmail subscriptionEmail);

        /// <summary>
        /// Удаляет email-адрес из базы данных подписок по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}