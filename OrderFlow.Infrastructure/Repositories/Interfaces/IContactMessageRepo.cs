using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления сообщениями обратной связи (ContactMessages)
    /// с поддержкой асинхронности и быстрой выборки по индексам состояния.
    /// </summary>
    public interface IContactMessageRepo
    {
        /// <summary>
        /// Добавляет новое входящее сообщение с сайта.
        /// </summary>
        Task<ContactMessage> CreateAsync(ContactMessage message);

        /// <summary>
        /// Возвращает сообщение по его уникальному идентификатору (Id). Если не найдено — возвращает null.
        /// </summary>
        Task<ContactMessage?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех сообщений, отсортированных по дате создания (от новых к старым).
        /// </summary>
        Task<IEnumerable<ContactMessage>> GetAllAsync();

        /// <summary>
        /// Возвращает список только непрочитанных сообщений для оперативной обработки администратором.
        /// </summary>
        Task<IEnumerable<ContactMessage>> GetUnreadAsync();

        /// <summary>
        /// Обновляет данные сообщения (используется для пометки IsRead или фиксации даты ответа RepliedAt).
        /// </summary>
        Task UpdateAsync(ContactMessage message);

        /// <summary>
        /// Удаляет сообщение по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}