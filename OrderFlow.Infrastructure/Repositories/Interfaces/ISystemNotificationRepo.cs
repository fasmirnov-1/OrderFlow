using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления системными уведомлениями и системными логами (SystemNotifications)
    /// с поддержкой индексированных быстрых выборок для административной панели.
    /// </summary>
    public interface ISystemNotificationRepo
    {
        /// <summary>
        /// Добавляет новое системное уведомление.
        /// </summary>
        Task<SystemNotification> CreateAsync(SystemNotification notification);

        /// <summary>
        /// Возвращает уведомление по его целочисленному идентификатору (Id). Если не найдено — возвращает null.
        /// </summary>
        Task<SystemNotification?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех системных уведомлений, отсортированных от новых к старым (AsNoTracking).
        /// </summary>
        Task<IEnumerable<SystemNotification>> GetAllAsync();

        /// <summary>
        /// Возвращает список только непрочитанных административных уведомлений (для вывода счетчиков в шапке панели).
        /// </summary>
        Task<IEnumerable<SystemNotification>> GetUnreadAdminNotificationsAsync();

        /// <summary>
        /// Обновляет состояние уведомления (используется для изменения флага IsRead).
        /// </summary>
        Task UpdateAsync(SystemNotification notification);

        /// <summary>
        /// Удаляет системное уведомление из базы данных по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}