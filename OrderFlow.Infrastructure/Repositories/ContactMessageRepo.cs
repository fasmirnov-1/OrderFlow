using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IContactMessageRepo
    public class ContactMessageRepo : IContactMessageRepo
    {
        private readonly AppDbContext _context;

        public ContactMessageRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<ContactMessage?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип, так как запись может отсутствовать
            return await _context.ContactMessages.FindAsync(id);
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списков в админ-панели
            // Сортируем по индексу CreatedAt (сначала новые)
            return await _context.ContactMessages
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactMessage>> GetUnreadAsync()
        {
            // Эффективная выборка по индексу IsRead для вывода уведомлений
            return await _context.ContactMessages
                .AsNoTracking()
                .Where(m => !m.IsRead)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(ContactMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var message = await _context.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}