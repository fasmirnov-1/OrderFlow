using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления статьями блога (BlogPosts)
    /// с контролем уникальности URL-слагов и валидацией внешних ключей категорий.
    /// </summary>
    public interface IBlogPostRepo
    {
        /// <summary>
        /// Создает новый пост блога с превентивной валидацией Slug и CategoryId.
        /// </summary>
        Task<BlogPost> CreateAsync(BlogPost blogPost);

        /// <summary>
        /// Возвращает пост по его идентификатору, включая связанные данные категории. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<BlogPost?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех постов блога со связанными категориями без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<BlogPost>> GetAllAsync();

        /// <summary>
        /// Обновляет существующий пост, проверяя ограничения уникальности слага среди других постов.
        /// </summary>
        Task UpdateAsync(BlogPost blogPost);

        /// <summary>
        /// Удаляет пост блога по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}