using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс SeoSettingRepo, осуществляющий crud операции над сущностью SeoSetting с учетом имеющихся констрент и ключей.
    public class SeoSettingRepo
    {
        private AppDbContext _context;

        public SeoSettingRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SeoSetting> CreateAsync(SeoSetting seoSetting)
        {
            _context.SeoSettings.Add(seoSetting);
            await _context.SaveChangesAsync();
            return seoSetting;
        }

        public async Task<SeoSetting> GetByIdAsync(int id)
        {
            return await _context.SeoSettings.FindAsync(id);
        }

        public async Task<IEnumerable<SeoSetting>> GetAllAsync()
        {
            return await _context.SeoSettings.ToListAsync();
        }

        public async Task UpdateAsync(SeoSetting seoSetting)
        {
            _context.SeoSettings.Update(seoSetting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var seoSetting = await _context.SeoSettings.FindAsync(id);
            if (seoSetting != null)
            {
                _context.SeoSettings.Remove(seoSetting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
