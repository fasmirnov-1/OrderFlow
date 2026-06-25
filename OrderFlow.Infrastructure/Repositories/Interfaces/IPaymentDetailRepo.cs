using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления платежными реквизитами и методами оплаты (PaymentDetails)
    /// </summary>
    public interface IPaymentDetailRepo
    {
        /// <summary>
        /// Добавляет новый набор платежных реквизитов в систему.
        /// </summary>
        Task<PaymentDetail> CreateAsync(PaymentDetail paymentDetail);

        /// <summary>
        /// Возвращает платежные реквизиты по идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<PaymentDetail?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех платежных реквизитов без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<PaymentDetail>> GetAllAsync();

        /// <summary>
        /// Обновляет существующие данные платежных реквизитов.
        /// </summary>
        Task UpdateAsync(PaymentDetail paymentDetail);

        /// <summary>
        /// Удаляет платежные реквизиты из системы по их идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}