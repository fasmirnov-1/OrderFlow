using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления глобальными конфигурациями и контактными данными сайта (SiteSettings)
    /// с автоматическим контролем штампов времени изменений.
    /// </summary>
    public interface ISiteSettingRepo
    {
        /// <summary>
        /// Инициализирует и сохраняет новый набор настроек сайта.
        /// </summary>
        Task<SiteSetting> CreateAsync(SiteSetting siteSetting);

        /// <summary>
        /// Возвращает конфигурацию по ее уникальному идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<SiteSetting?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает текущие актуальные настройки сайта (самую свежую запись по дате обновления) без отслеживания изменений.
        /// Используется повсеместно на фронтенде (футер, контакты, шапка сайта).
        /// </summary>
        Task<SiteSetting?> GetCurrentSettingsAsync();

        /// <summary>
        /// Возвращает полный список всех созданных записей настроек без отслеживания изменений.
        /// </summary>
        Task<IEnumerable<SiteSetting>> GetAllAsync();

        /// <summary>
        /// Обновляет параметры конфигурации сайта с фиксацией точного времени изменения в поле UpdatedAt.
        /// </summary>
        Task UpdateAsync(SiteSetting siteSetting);

        /// <summary>
        /// Удаляет настройки сайта из системы по идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}