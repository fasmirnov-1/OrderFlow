using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс EmailTemplateRepo, осуществляющий crud операции над сущностью EmailTemplate с учетом имеющихся констрент и ключей.
    public class EmailTemplateRepo
    {
        private AppDbContext _context;
        public EmailTemplateRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();
            return emailTemplate;
        }
        public async Task<EmailTemplate> GetByIdAsync(Guid id)
        {
            return await _context.EmailTemplates.FindAsync(id);
        }
        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            return await _context.EmailTemplates.ToListAsync();
        }
        public async Task UpdateAsync(EmailTemplate emailTemplate)
        {
            _context.EmailTemplates.Update(emailTemplate);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            if (emailTemplate != null)
            {
                _context.EmailTemplates.Remove(emailTemplate);
                await _context.SaveChangesAsync();
            }
        }
    }
}
