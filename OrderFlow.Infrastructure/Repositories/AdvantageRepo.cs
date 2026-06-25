using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class AdvantageRepo : IAdvantageRepo
    {
        private readonly AppDbContext _context;
        private bool _disposed = false;

        public AdvantageRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Advantage> CreateAsync(Advantage advantage)
        {
            if (advantage == null) throw new ArgumentNullException(nameof(advantage));

            // ПРОВЕРКА УНИКАЛЬНОСТИ (Уникальный индекс/Бизнес-констреинт на Title)
            bool isTitleUnique = !await _context.Advantages.AnyAsync(x => x.Title == advantage.Title);
            if (!isTitleUnique)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Преимущество с заголовком '{advantage.Title}' уже существует.");
            }

            await _context.Advantages.AddAsync(advantage);
            await _context.SaveChangesAsync();
            return advantage;
        }

        public async Task<Advantage?> ReadAsync(int id)
        {
            return await _context.Advantages.FindAsync(id);
        }

        public async Task<IEnumerable<Advantage>> ReadAllAsync()
        {
            return await _context.Advantages.AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(Advantage advantage)
        {
            if (advantage == null) throw new ArgumentNullException(nameof(advantage));

            // Проверка уникальности заголовка при изменении (исключая саму эту запись)
            bool isTitleUnique = !await _context.Advantages.AnyAsync(x => x.Title == advantage.Title && x.Id != advantage.Id);
            if (!isTitleUnique)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Заголовок '{advantage.Title}' уже занят другой записью.");
            }

            _context.Advantages.Update(advantage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var advantage = await _context.Advantages.FindAsync(id);
            if (advantage != null)
            {
                _context.Advantages.Remove(advantage);
                await _context.SaveChangesAsync();
            }
        }
    }
}