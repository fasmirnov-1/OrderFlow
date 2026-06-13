using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс SiteSettingRepo, осуществляющий crud операции над сущностью SiteSetting с учетом имеющихся констрент и ключей.
    public class SiteSettingRepo
    {
        private AppDbContext _context;

        public SiteSettingRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SiteSetting> CreateAsync(SiteSetting siteSetting)
        {
            _context.SiteSettings.Add(siteSetting);
            await _context.SaveChangesAsync();
            return siteSetting;
        }

        public async Task<SiteSetting> GetByIdAsync(int id)
        {
            return await _context.SiteSettings.FindAsync(id);
        }

        public async Task<IEnumerable<SiteSetting>> GetAllAsync()
        {
            return await _context.SiteSettings.ToListAsync();
        }

        public async Task UpdateAsync(SiteSetting siteSetting)
        {
            _context.SiteSettings.Update(siteSetting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var siteSetting = await _context.SiteSettings.FindAsync(id);
            if (siteSetting != null)
            {
                _context.SiteSettings.Remove(siteSetting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
