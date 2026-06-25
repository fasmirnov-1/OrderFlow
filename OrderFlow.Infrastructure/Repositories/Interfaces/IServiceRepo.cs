using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления услугами (Services)
    /// с учетом кастомной сортировки и фильтрации активных позиций.
    /// </summary>
    public interface IServiceRepo
    {
        /// <summary>
        /// Добавляет новую услугу в систему.
        /// </summary>
        Task<Service> CreateAsync(Service service);

        /// <summary>
        /// Находит услугу по ее уникальному целочисленному идентификатору (Id). Если не найдена — возвращает null.
        /// </summary>
        Task<Service?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех услуг (включая неактивные), отсортированных по DisplayOrder.
        /// </summary>
        Task<IEnumerable<Service>> GetAllAsync();

        /// <summary>
        /// Возвращает список только активных услуг для отображения пользователям на сайте.
        /// </summary>
        Task<IEnumerable<Service>> GetActiveAsync();

        /// <summary>
        /// Обновляет параметры и состояние существующей услуги.
        /// </summary>
        Task UpdateAsync(Service service);

        /// <summary>
        /// Удаляет услугу из системы по ее идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}