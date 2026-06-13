using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс ServiceRepo, осуществляющий crud операции над сущностью Service с учетом имеющихся констрент и ключей.
    public class ServiceRepo
    {
        private AppDbContext _context;

        public ServiceRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Service> CreateAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<Service> GetByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<List<Service>> GetAllAsync()
        {
            var services = await _context.Services.ToListAsync();
            return services;
        }

        public async Task UpdateAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }
}
