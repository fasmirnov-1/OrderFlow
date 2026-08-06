using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    public class UserSessionsRepo : IUserSessionsRepo
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<UserSession> _dbSet;

        public UserSessionsRepo(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<UserSession>();
        }

        public async Task<UserSession?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<UserSession?> GetByHashAsync(string sessionHash)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionHash == sessionHash);
        }

        public async Task AddAsync(UserSession entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(UserSession entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(UserSession entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<UserSession>> GetExpiredSessionsAsync(DateTime utcNow)
        {
            return await _dbSet
                .Where(s => s.ExpiresAt <= utcNow)
                .ToListAsync();
        }
    }
}
