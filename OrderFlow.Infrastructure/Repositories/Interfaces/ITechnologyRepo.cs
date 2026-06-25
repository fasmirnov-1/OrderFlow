using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления технологическим стеком (Technologies)
    /// с поддержкой безопасной очистки связей Many-to-Many в проектах портфолио.
    /// </summary>
    public interface ITechnologyRepo
    {
        /// <summary>
        /// Добавляет новую технологию в систему.
        /// </summary>
        Task<Technology> CreateAsync(Technology technology);

        /// <summary>
        /// Возвращает технологию по ее целочисленному идентификатору (Id) вместе со списком связанных проектов.
        /// </summary>
        Task<Technology?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех доступных технологий без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<Technology>> GetAllAsync();

        /// <summary>
        /// Обновляет параметры существующей технологии.
        /// </summary>
        Task UpdateAsync(Technology technology);

        /// <summary>
        /// Безопасно удаляет технологию из системы, автоматически очищая связанные записи в связующей таблице проектов.
        /// </summary>
        Task DeleteAsync(int id);
    }
}