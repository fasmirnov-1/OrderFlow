using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс TestimonialRepo, осуществляющий crud операции над сущностью Testimonial с учетом имеющихся констрент и ключей.
    public class TestimonialRepo
    {
        private AppDbContext _context;

        public TestimonialRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Testimonial> CreateAsync(Testimonial testimonial)
        {
            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();
            return testimonial;
        }

        public async Task<Testimonial> GetByIdAsync(int id)
        {
            return await _context.Testimonials.FindAsync(id);
        }

        public async Task<IEnumerable<Testimonial>> GetAllAsync()
        {
            return await _context.Testimonials.ToListAsync();
        }
        public async Task<IEnumerable<Testimonial>> GetLimited(int Limit)
        {
            return await _context.Testimonials.Take(Limit).ToListAsync();
        }

        public async Task UpdateAsync(Testimonial testimonial)
        {
            _context.Testimonials.Update(testimonial);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var testimonial = await GetByIdAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
                await _context.SaveChangesAsync();
            }
        }
    }
}
