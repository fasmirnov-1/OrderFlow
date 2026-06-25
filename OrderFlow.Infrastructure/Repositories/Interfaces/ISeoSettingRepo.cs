using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления SEO-настройками страниц (SeoSettings)
    /// с контролем уникальности маршрутов и оптимизацией кеширования метаданных сайта index-роботами.
    /// </summary>
    public interface ISeoSettingRepo
    {
        /// <summary>
        /// Создает новый блок SEO-настроек для страницы с проверкой уникальности PageRoute.
        /// </summary>
        Task<SeoSetting> CreateAsync(SeoSetting seoSetting);

        /// <summary>
        /// Возвращает SEO-настройки по первичному ключу. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<SeoSetting?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает мета-данные и SEO параметры по относительному или абсолютному URL/маршруту страницы (использует уникальный индекс).
        /// </summary>
        /// <param name="pageRoute">Маршрут страницы (например, "/portfolio" или "/contacts").</param>
        Task<SeoSetting?> GetByRouteAsync(string pageRoute);

        /// <summary>
        /// Возвращает полный список настроенных страниц без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<SeoSetting>> GetAllAsync();

        /// <summary>
        /// Обновляет существующие SEO-параметры страницы, предотвращая коллизии дублирования роутов.
        /// </summary>
        Task UpdateAsync(SeoSetting seoSetting);

        /// <summary>
        /// Удаляет настройки мета-данных страницы по идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}