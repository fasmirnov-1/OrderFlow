using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AspNetUserLoginRepo, который выполянет crud опеарции для сущности AspNetUserLogin с учетом контреинт и клчей.
    public class AspNetUserLoginRepo
    {
        private AppDbContext _context;

        public AspNetUserLoginRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AspNetUserLogin entity)
        {
            _context.AspNetUserLogins.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<AspNetUserLogin> ReadAsync(string userId, string loginProvider, string providerKey)
        {
            return await _context.AspNetUserLogins.FindAsync(userId, loginProvider, providerKey);
        }

        public async Task UpdateAsync(AspNetUserLogin entity)
        {
            _context.AspNetUserLogins.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId, string loginProvider, string providerKey)
        {
            var entity = await ReadAsync(userId, loginProvider, providerKey);
            if (entity != null)
            {
                _context.AspNetUserLogins.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
