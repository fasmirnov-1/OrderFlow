using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс ContactMessageRepo, выполняющий crud операции для сущности ContactMessage. Учесть дейсвующие констренты и ключи.
    public class ContactMessageRepo
    {
        private AppDbContext _context;
        public ContactMessageRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ContactMessage> CreateAsync(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task<ContactMessage> GetByIdAsync(int id)
        {
            return await _context.ContactMessages.FindAsync(id);
        }
        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await _context.ContactMessages.ToListAsync();
        }
        public async Task UpdateAsync(ContactMessage message)
        {
            _context.ContactMessages.Update(message);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}
