using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ISubscriptionEmailRepo
    public class SubscriptionEmailRepo : ISubscriptionEmailRepo
    {
        private readonly AppDbContext _context;

        public SubscriptionEmailRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SubscriptionEmail> CreateAsync(SubscriptionEmail subscriptionEmail)
        {
            if (subscriptionEmail == null) throw new ArgumentNullException(nameof(subscriptionEmail));

            // 1. ПРОВЕРКА УНИКАЛЬНОСТИ EMAIL
            bool emailExists = await _context.SubscriptionEmails.AnyAsync(s => s.Email == subscriptionEmail.Email);
            if (emailExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Email '{subscriptionEmail.Email}' уже подписан на рассылку.");
            }

            // 2. ГЕНЕРАЦИЯ И ПРОВЕРКА УНИКАЛЬНОСТИ UNSUBSCRIBE TOKEN (если не передан извне)
            if (string.IsNullOrWhiteSpace(subscriptionEmail.UnsubscribeToken))
            {
                subscriptionEmail.UnsubscribeToken = Guid.NewGuid().ToString("N");
            }
            else
            {
                bool tokenExists = await _context.SubscriptionEmails.AnyAsync(s => s.UnsubscribeToken == subscriptionEmail.UnsubscribeToken);
                if (tokenExists)
                {
                    throw new ArgumentException("Нарушение Unique Constraint: Сгенерированный UnsubscribeToken уже существует в базе данных.");
                }
            }

            // Инициализация даты подписки, если не задана
            if (subscriptionEmail.SubscribedAt == default)
            {
                subscriptionEmail.SubscribedAt = DateTime.UtcNow;
            }

            _context.SubscriptionEmails.Add(subscriptionEmail);
            await _context.SaveChangesAsync();
            return subscriptionEmail;
        }

        public async Task<SubscriptionEmail?> GetByIdAsync(int id)
        {
            return await _context.SubscriptionEmails.FindAsync(id);
        }

        public async Task<SubscriptionEmail?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            // Быстрый поиск по индексу IX_SubscriptionEmails_Email
            return await _context.SubscriptionEmails
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<SubscriptionEmail?> GetByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            // Быстрый поиск по индексу IX_SubscriptionEmails_UnsubscribeToken для процедуры отписки
            return await _context.SubscriptionEmails
                .FirstOrDefaultAsync(s => s.UnsubscribeToken == token);
        }

        public async Task<IEnumerable<SubscriptionEmail>> GetAllAsync()
        {
            // Используем AsNoTracking() для тяжелых списков адресов в панели управления
            return await _context.SubscriptionEmails
                .AsNoTracking()
                .OrderByDescending(s => s.SubscribedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubscriptionEmail>> GetActiveSubscriptionsAsync()
        {
            // Выборка только активных подписчиков по индексу IX_SubscriptionEmails_IsActive для отправки писем
            return await _context.SubscriptionEmails
                .AsNoTracking()
                .Where(s => s.IsActive == true)
                .ToListAsync();
        }

        public async Task UpdateAsync(SubscriptionEmail subscriptionEmail)
        {
            if (subscriptionEmail == null) throw new ArgumentNullException(nameof(subscriptionEmail));

            // ПРОВЕРКА УНИКАЛЬНОСТИ EMAIL ПРИ ОБНОВЛЕНИИ (Исключая текущую запись)
            bool emailExists = await _context.SubscriptionEmails
                .AnyAsync(s => s.Email == subscriptionEmail.Email && s.Id != subscriptionEmail.Id);
            if (emailExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint при обновлении: Email '{subscriptionEmail.Email}' уже занят другим подписчиком.");
            }

            // ПРОВЕРКА УНИКАЛЬНОСТИ ТОКЕНА ПРИ ОБНОВЛЕНИИ
            bool tokenExists = await _context.SubscriptionEmails
                .AnyAsync(s => s.UnsubscribeToken == subscriptionEmail.UnsubscribeToken && s.Id != subscriptionEmail.Id);
            if (tokenExists)
            {
                throw new ArgumentException("Нарушение Unique Constraint при обновлении: Данный UnsubscribeToken уже используется в другой записи.");
            }

            _context.Entry(subscriptionEmail).State = EntityState.Modified;

            // Защищаем дату подписки от случайного затирания или изменения
            _context.Entry(subscriptionEmail).Property(x => x.SubscribedAt).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subscriptionEmail = await _context.SubscriptionEmails.FirstOrDefaultAsync(s => s.Id == id);
            if (subscriptionEmail != null)
            {
                _context.SubscriptionEmails.Remove(subscriptionEmail);
                await _context.SaveChangesAsync();
            }
        }
    }
}