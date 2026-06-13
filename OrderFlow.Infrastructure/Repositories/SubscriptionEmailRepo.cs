using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс SubscriptionEmailRepo, осуществляющий crud операции над сущностью SubscriptionEmail с учетом имеющихся констрент и ключей.
    public class SubscriptionEmailRepo
    {
        private AppDbContext _context;

        public SubscriptionEmailRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionEmail> CreateAsync(SubscriptionEmail subscriptionEmail)
        {
            _context.SubscriptionEmails.Add(subscriptionEmail);
            await _context.SaveChangesAsync();
            return subscriptionEmail;
        }

        public async Task<SubscriptionEmail> GetByIdAsync(int id)
        {
            return await _context.SubscriptionEmails.FindAsync(id);
        }

        public async Task<IEnumerable<SubscriptionEmail>> GetAllAsync()
        {
            return await _context.SubscriptionEmails.ToListAsync();
        }

        public async Task UpdateAsync(SubscriptionEmail subscriptionEmail)
        {
            _context.SubscriptionEmails.Update(subscriptionEmail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subscriptionEmail = await GetByIdAsync(id);
            if (subscriptionEmail != null)
            {
                _context.SubscriptionEmails.Remove(subscriptionEmail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
