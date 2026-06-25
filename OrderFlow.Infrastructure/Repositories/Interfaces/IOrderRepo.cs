using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления заказами (Orders) 
    /// с оптимизацией выборок по индексам статуса, даты и электронной почты.
    /// </summary>
    public interface IOrderRepo
    {
        /// <summary>
        /// Создает новый заказ в системе.
        /// </summary>
        Task<Order> CreateOrderAsync(Order order);

        /// <summary>
        /// Находит заказ по его уникальному идентификатору (Id). Если не найден — возвращает null.
        /// </summary>
        Task<Order?> GetOrderByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех заказов в системе, отсортированных от новых к старым.
        /// </summary>
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        /// <summary>
        /// Возвращает список заказов с определенным статусом (использует индекс).
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(int status);

        /// <summary>
        /// Находит все заказы, оформленные на конкретный Email (использует индекс).
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByEmailAsync(string email);

        /// <summary>
        /// Обновляет параметры и состояние существующего заказа.
        /// </summary>
        Task UpdateOrderAsync(Order order);

        /// <summary>
        /// Удаляет заказ по его идентификатору.
        /// </summary>
        Task DeleteOrderAsync(int id);
    }
}