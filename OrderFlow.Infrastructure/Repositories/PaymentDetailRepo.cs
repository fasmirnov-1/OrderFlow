using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IPaymentDetailRepo
    public class PaymentDetailRepo : IPaymentDetailRepo
    {
        private readonly AppDbContext _context;

        public PaymentDetailRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<PaymentDetail> CreateAsync(PaymentDetail paymentDetail)
        {
            if (paymentDetail == null) throw new ArgumentNullException(nameof(paymentDetail));

            _context.PaymentDetails.Add(paymentDetail);
            await _context.SaveChangesAsync();
            return paymentDetail;
        }

        public async Task<PaymentDetail?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай, если запись удалена или отсутствует
            return await _context.PaymentDetails.FindAsync(id);
        }

        public async Task<IEnumerable<PaymentDetail>> GetAllAsync()
        {
            // Применяем AsNoTracking() для оптимизации запросов на чтение списков реквизитов
            return await _context.PaymentDetails
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(PaymentDetail paymentDetail)
        {
            if (paymentDetail == null) throw new ArgumentNullException(nameof(paymentDetail));

            // Используем явную установку флага Modified вместо Update(), 
            // чтобы минимизировать избыточные SQL-апдейты нединамических полей
            _context.Entry(paymentDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            // Оптимизируем поиск перед удалением через FirstOrDefaultAsync
            var paymentDetail = await _context.PaymentDetails.FirstOrDefaultAsync(p => p.Id == id);
            if (paymentDetail != null)
            {
                _context.PaymentDetails.Remove(paymentDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}