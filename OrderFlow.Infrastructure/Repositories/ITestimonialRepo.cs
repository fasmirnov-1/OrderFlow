using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для управления отзывами клиентов (Testimonials)
    /// с поддержкой лимитированных и промодерированных выборок контента для фронтенда.
    /// </summary>
    public interface ITestimonialRepo
    {
        /// <summary>
        /// Добавляет новый отзыв в систему (например, отправленный клиентом через форму на сайте).
        /// </summary>
        Task<Testimonial> CreateAsync(Testimonial testimonial);

        /// <summary>
        /// Находит отзыв по его идентификатору. Если запись отсутствует — возвращает null.
        /// </summary>
        Task<Testimonial?> GetByIdAsync(int id);

        /// <summary>
        /// Возвращает полный список всех отзывов в системе без отслеживания изменений (AsNoTracking).
        /// </summary>
        Task<IEnumerable<Testimonial>> GetAllAsync();

        /// <summary>
        /// Возвращает ограниченное количество отзывов без отслеживания изменений (используется для слайдеров/каруселей).
        /// </summary>
        Task<IEnumerable<Testimonial>> GetLimitedAsync(int limit);

        /// <summary>
        /// Возвращает только одобренные администратором отзывы для безопасного вывода на публичных страницах.
        /// </summary>
        Task<IEnumerable<Testimonial>> GetApprovedAsync(int? limit = null);

        /// <summary>
        /// Обновляет параметры и статус модерации существующего отзыва.
        /// </summary>
        Task UpdateAsync(Testimonial testimonial);

        /// <summary>
        /// Удаляет отзыв из системы по его идентификатору.
        /// </summary>
        Task DeleteAsync(int id);
    }
}