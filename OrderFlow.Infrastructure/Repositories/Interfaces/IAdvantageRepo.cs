using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories.Interfaces
{
    public interface IAdvantageRepo
    {
        Task<Advantage> CreateAsync(Advantage advantage);
        Task<Advantage?> ReadAsync(int id);
        Task<IEnumerable<Advantage>> ReadAllAsync();
        Task UpdateAsync(Advantage advantage);
        Task DeleteAsync(int id);
    }
}