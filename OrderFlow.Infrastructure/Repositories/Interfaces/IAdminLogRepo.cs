using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    public interface IAdminLogRepo
    {
        Task<AdminLog> CreateAsync(AdminLog adminLog);
        Task<AdminLog?> GetByIdAsync(int id);
        Task<IEnumerable<AdminLog>> GetAllAsync();
        Task UpdateAsync(AdminLog adminLog);
        Task DeleteAsync(int id);
    }
}