using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    public interface IUserSessionsRepo
    {
        Task<UserSession?> GetByIdAsync(int id);
        Task<UserSession?> GetByHashAsync(string sessionHash);
        Task AddAsync(UserSession entity);
        void Update(UserSession entity);
        void Remove(UserSession entity);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<UserSession>> GetExpiredSessionsAsync(DateTime utcNow);
    }
}
