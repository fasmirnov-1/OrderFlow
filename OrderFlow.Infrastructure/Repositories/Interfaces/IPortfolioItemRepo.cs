using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления проектами портфолио (PortfolioItems)
    /// с поддержкой загрузки категорий Many-to-Many и фильтрации по индексам.
    /// </summary>
    public interface IPortfolioItemRepo
    {
        /// <summary>
        /// Добавляет новый проект в портфолио.
        /// </summary>
        Task<PortfolioItem> CreateAsync(PortfolioItem item);

        /// <summary>
        /// Находит проект по его целочисленному идентификатору (Id) вместе со всеми привязанными категориями.
        /// </summary>
        Task<PortfolioItem?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех проектов со связанными категориями, отсортированных по дате создания.
        /// </summary>
        Task<IEnumerable<PortfolioItem>> GetAllAsync();

        /// <summary>
        /// Возвращает список только опубликованных проектов для вывода на публичную часть сайта.
        /// </summary>
        Task<IEnumerable<PortfolioItem>> GetPublishedAsync();

        /// <summary>
        /// Обновляет параметры существующего проекта портфолио.
        /// </summary>
        Task UpdateAsync(PortfolioItem item);

        /// <summary>
        /// Безопасно удаляет проект портфолио, включая очистку связей "многие-ко-многим".
        /// </summary>
        Task DeleteAsync(int id);
    }
}