using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс TechnologyRepo, осуществляющий crud операции над сущностью Technology с учетом имеющихся констрент и ключей.
    public class TechnologyRepo
    {
        private AppDbContext _context;

        public TechnologyRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Technology> CreateAsync(Technology technology)
        {
            _context.Technologies.Add(technology);
            await _context.SaveChangesAsync();
            return technology;
        }

        public async Task<Technology> GetByIdAsync(int id)
        {
            return await _context.Technologies.FindAsync(id);
        }

        public async Task<IEnumerable<Technology>> GetAllAsync()
        {
            return await _context.Technologies.ToListAsync();
        }

        public async Task UpdateAsync(Technology technology)
        {
            _context.Technologies.Update(technology);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var technology = await _context.Technologies.FindAsync(id);
            if (technology != null)
            {
                _context.Technologies.Remove(technology);
                await _context.SaveChangesAsync();
            }
        }
    }
}
