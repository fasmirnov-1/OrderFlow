using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс PaymentDetailRepo, осуществляющий crud операции над сущностью PaymentDetail с учетом имеющихся констрент и ключей.
    public class PaymentDetailRepo
    {
        private AppDbContext _context;

        public PaymentDetailRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDetail> CreateAsync(PaymentDetail paymentDetail)
        {
            _context.PaymentDetails.Add(paymentDetail);
            await _context.SaveChangesAsync();
            return paymentDetail;
        }

        public async Task<PaymentDetail> GetByIdAsync(int id)
        {
            return await _context.PaymentDetails.FindAsync(id);
        }

        public async Task<IEnumerable<PaymentDetail>> GetAllAsync()
        {
            return await _context.PaymentDetails.ToListAsync();
        }

        public async Task UpdateAsync(PaymentDetail paymentDetail)
        {
            _context.PaymentDetails.Update(paymentDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var paymentDetail = await _context.PaymentDetails.FindAsync(id);
            if (paymentDetail != null)
            {
                _context.PaymentDetails.Remove(paymentDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
