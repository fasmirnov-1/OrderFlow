using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс ITagRepo
    public class TagRepo : ITagRepo
    {
        private readonly AppDbContext _context;

        public TagRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Tag> CreateTagAsync(Tag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ИМЕНИ
            bool nameExists = await _context.Tags.AnyAsync(t => t.Name == tag.Name);
            if (nameExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Тег с именем '{tag.Name}' уже существует.");
            }

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ СЛАГА (URL-маршрута)
            bool slugExists = await _context.Tags.AnyAsync(t => t.Slug == tag.Slug);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Тег со слагом '{tag.Slug}' уже существует.");
            }

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            // Возвращаем nullable-тип, подгружая связанные статьи (при необходимости детального просмотра)
            return await _context.Tags
                .Include(t => t.BlogPosts)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tag?> GetBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            // Быстрый поиск по индексу IX_Tags_Slug для фильтрации статей блога на фронтенде
            return await _context.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода облака тегов или таблиц в админке
            return await _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ИМЕНИ ПРИ ОБНОВЛЕНИИ (Исключая текущий тег)
            bool nameExists = await _context.Tags.AnyAsync(t => t.Name == tag.Name && t.Id != tag.Id);
            if (nameExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Имя тега '{tag.Name}' уже занято другим тегом.");
            }

            // ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ СЛАГА ПРИ ОБНОВЛЕНИИ (Исключая текущий тег)
            bool slugExists = await _context.Tags.AnyAsync(t => t.Slug == tag.Slug && t.Id != tag.Id);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: URL-слаг '{tag.Slug}' уже используется другим тегом.");
            }

            _context.Entry(tag).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            // Извлекаем тег вместе с коллекцией связей Many-to-Many
            var tag = await _context.Tags
                .Include(t => t.BlogPosts)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag != null)
            {
                // Перед удалением сущности очищаем коллекцию навигационного свойства,
                // чтобы EF Core корректно сгенерировал DELETE-запросы для промежуточной таблицы связей
                tag.BlogPosts.Clear();

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}