using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AspNetUserClaimRepo, который будет выполнять crud операции для сущности AspNetUserClaim и реализовывать интерфейс IAspNetUserClaimRepo с учетом имеющихся констрент и ключей.
    public class AspNetUserClaimRepo
    {
        private AppDbContext _context;
        public AspNetUserClaimRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AspNetUserClaim> CreateAsync(AspNetUserClaim entity)
        {
            _context.AspNetUserClaims.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.AspNetUserClaims.FindAsync(id);
            if (entity != null)
            {
                _context.AspNetUserClaims.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<AspNetUserClaim>> GetAllAsync()
        {
            return await _context.AspNetUserClaims.ToListAsync();
        }
        public async Task<AspNetUserClaim> GetByIdAsync(int id)
        {
            return await _context.AspNetUserClaims.FindAsync(id);
        }
        public async Task UpdateAsync(AspNetUserClaim entity)
        {
            _context.AspNetUserClaims.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
