using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AspNetUser, который реализует интерфейс IAspNetUser и выполняет crud операции для сущности AspAspNetUser в базе данных с учетом всех имеющихся констрентов и ключей.
    public class AspNetUserRepo
    {
        private AppDbContext _context;
        public AspNetUserRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AspNetUser> CreateAsync(AspNetUser user)
        {
            _context.AspNetUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<AspNetUser> GetByIdAsync(string id)
        {
            return await _context.AspNetUsers.FindAsync(id);
        }
        public async Task<IEnumerable<AspNetUser>> GetAllAsync()
        {
            return await _context.AspNetUsers.ToListAsync();
        }
        public async Task UpdateAsync(AspNetUser user)
        {
            _context.AspNetUsers.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(string id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.AspNetUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
