using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления категориями блога (BlogCategories)
    /// с контролем уникальности слагов и превентивной проверкой ссылочной целостности.
    /// </summary>
    public interface IBlogCategoryRepo
    {
        /// <summary>
        /// Создает новую категорию блога с валидацией уникальности Slug.
        /// </summary>
        Task<BlogCategory> CreateAsync(BlogCategory category);

        /// <summary>
        /// Возвращает категорию по ее идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<BlogCategory?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех категорий без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<BlogCategory>> GetAllAsync();

        /// <summary>
        /// Обновляет существующую категорию, предотвращая коллизии уникальности Slug с другими записями.
        /// </summary>
        Task UpdateAsync(BlogCategory category);

        /// <summary>
        /// Удаляет категорию по идентификатору, блокируя операцию, если в ней присутствуют связанные посты.
        /// </summary>
        Task DeleteAsync(int id);
    }
}