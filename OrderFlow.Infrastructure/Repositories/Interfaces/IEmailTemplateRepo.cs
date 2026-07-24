using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления шаблонами писем (EmailTemplates)
    /// с контролем уникальности имен шаблонов.
    /// </summary>
    public interface IEmailTemplateRepo
    {
        /// <summary>
        /// Создает новый шаблон письма с валидацией уникальности его системного имени (Name).
        /// </summary>
        Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate);

        /// <summary>
        /// Возвращает шаблон письма по его уникальному целочисленному идентификатору (Id). Если не найден — возвращает null.
        /// </summary>
        Task<EmailTemplate?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает шаблон письма по его системному имени (использует уникальный индекс).
        /// </summary>
        Task<EmailTemplate?> GetByNameAsync(string name);

        /// <summary>
        /// Возвращает полный список всех доступных шаблонов без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<EmailTemplate>> GetAllAsync();

        /// <summary>
        /// Обновляет параметры существующего шаблона письма, предотвращая коллизии уникальности системных имен.
        /// </summary>
        Task UpdateAsync(EmailTemplate emailTemplate);

        /// <summary>
        /// Удаляет шаблон письма по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
        Task<EmailTemplate?> GetActiveTemplateByNameAsync(string templateName);
    }
}