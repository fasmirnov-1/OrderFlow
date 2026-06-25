using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления категориями портфолио (PortfolioCategories)
    /// с валидацией уникальности URL-слагов и безопасным удалением.
    /// </summary>
    public interface IPortfolioCategoryRepo
    {
        /// <summary>
        /// Создает новую категорию портфолио с превентивной проверкой уникальности Slug.
        /// </summary>
        Task<PortfolioCategory> CreateAsync(PortfolioCategory category);

        /// <summary>
        /// Возвращает категорию по ее идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<PortfolioCategory?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех категорий портфолио без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<PortfolioCategory>> GetAllAsync();

        /// <summary>
        /// Обновляет параметры существующей категории портфолио, контролируя коллизии слагов.
        /// </summary>
        Task UpdateAsync(PortfolioCategory category);

        /// <summary>
        /// Удаляет категорию портфолио, предотвращая нарушение связей Many-to-Many, если к ней привязаны проекты.
        /// </summary>
        Task DeleteAsync(int id);
    }
}