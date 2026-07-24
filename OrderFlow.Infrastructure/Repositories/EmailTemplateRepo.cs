using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IEmailTemplateRepo
    public class EmailTemplateRepo : IEmailTemplateRepo
    {
        private readonly AppDbContext _context;

        public EmailTemplateRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate)
        {
            if (emailTemplate == null) throw new ArgumentNullException(nameof(emailTemplate));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ: Проверяем Name на дубликаты перед вставкой
            bool nameExists = await _context.EmailTemplates.AnyAsync(t => t.Name == emailTemplate.Name);
            if (nameExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Шаблон с именем '{emailTemplate.Name}' уже существует.");
            }

            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();
            return emailTemplate;
        }

        public async Task<EmailTemplate?> GetByIdAsync(int id)
        {
            // Используем тип int в соответствии со структурой сущности и возвращаем nullable-тип
            return await _context.EmailTemplates.FindAsync(id);
        }

        public async Task<EmailTemplate?> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            // Быстрый поиск по уникальному индексу Name для отправки уведомлений
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списков в админ-панели
            return await _context.EmailTemplates
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(EmailTemplate emailTemplate)
        {
            if (emailTemplate == null) throw new ArgumentNullException(nameof(emailTemplate));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущую запись)
            bool nameExists = await _context.EmailTemplates.AnyAsync(t => t.Name == emailTemplate.Name && t.Id != emailTemplate.Id);
            if (nameExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Имя шаблона '{emailTemplate.Name}' уже используется другим шаблоном.");
            }

            _context.Entry(emailTemplate).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id);
            if (emailTemplate != null)
            {
                _context.EmailTemplates.Remove(emailTemplate);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<EmailTemplate?> GetActiveTemplateByNameAsync(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException("Имя шаблона не может быть пустым.", nameof(templateName));
            }

            // Ищем шаблон по имени, у которого IsActive == true (true! учитывает nullable тип bool?)
            return await _context.EmailTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name == templateName && t.IsActive == true);
        }
    }
}