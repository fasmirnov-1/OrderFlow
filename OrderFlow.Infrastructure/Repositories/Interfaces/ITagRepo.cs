using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления тегами (Tags) 
    /// с жестким контролем уникальности имен, слагов и безопасным разрывом связей Many-to-Many.
    /// </summary>
    public interface ITagRepo
    {
        /// <summary>
        /// Создает новый тег с превентивной валидацией отсутствия дубликатов Name и Slug.
        /// </summary>
        Task<Tag> CreateTagAsync(Tag tag);

        /// <summary>
        /// Возвращает тег по его уникальному идентификатору (Id) вместе с коллекцией связанных статей.
        /// </summary>
        Task<Tag?> GetTagByIdAsync(int id);

        /// <summary>
        /// Находит тег по его уникальному URL-слагу (использует индекс для фильтрации контента на сайте).
        /// </summary>
        Task<Tag?> GetBySlugAsync(string slug);

        /// <summary>
        /// Возвращает полный список всех тегов, отсортированных по алфавиту без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<Tag>> GetAllTagsAsync();

        /// <summary>
        /// Обновляет параметры существующего тега, предотвращая коллизии уникальности имен или слагов.
        /// </summary>
        Task UpdateTagAsync(Tag tag);

        /// <summary>
        /// Безопасно удаляет тег из системы, автоматически очищая связанные записи в связующей таблице блогов.
        /// </summary>
        Task DeleteTagAsync(int id);
    }
}