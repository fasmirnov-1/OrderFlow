using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ITestimonialRepo
    public class TestimonialRepo : ITestimonialRepo
    {
        private readonly AppDbContext _context;

        public TestimonialRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Testimonial> CreateAsync(Testimonial testimonial)
        {
            if (testimonial == null) throw new ArgumentNullException(nameof(testimonial));

            // Автоматическое заполнение даты создания отзыва, если поле присутствует в вашей модели
            // testimonial.CreatedAt = DateTime.UtcNow;

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();
            return testimonial;
        }

        public async Task<Testimonial?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай, если отзыв отсутствует в БД
            return await _context.Testimonials.FindAsync(id);
        }

        public async Task<IEnumerable<Testimonial>> GetAllAsync()
        {
            // Применяем AsNoTracking() для тяжелых таблиц в панели администратора
            return await _context.Testimonials
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Testimonial>> GetLimitedAsync(int limit)
        {
            if (limit <= 0) return Enumerable.Empty<Testimonial>();

            // Оптимизируем чтение с помощью AsNoTracking(). 
            // Рекомендуется добавить сортировку, например по дате создания (.OrderByDescending(t => t.CreatedAt)) 
            // или приоритету отображения, чтобы лимитированная выборка возвращала актуальные данные.
            return await _context.Testimonials
                .AsNoTracking()
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<Testimonial>> GetApprovedAsync(int? limit = null)
        {
            // Специализированный метод для фронтенда — выводит только промодерированные отзывы.
            // Если в вашей модели поле называется по-другому (например, IsActive), замените t.IsApproved.
            var query = _context.Testimonials
                .AsNoTracking();
            // .Where(t => t.IsApproved == true); // Раскомментируйте при наличии флага модерации

            if (limit.HasValue && limit.Value > 0)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(Testimonial testimonial)
        {
            if (testimonial == null) throw new ArgumentNullException(nameof(testimonial));

            _context.Entry(testimonial).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Оптимизированный асинхронный поиск сущности перед удалением
            var testimonial = await _context.Testimonials.FirstOrDefaultAsync(t => t.Id == id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
                await _context.SaveChangesAsync();
            }
        }
    }
}