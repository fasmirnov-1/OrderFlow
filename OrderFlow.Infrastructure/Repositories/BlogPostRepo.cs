using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IBlogPostRepo
    public class BlogPostRepo : IBlogPostRepo
    {
        private readonly AppDbContext _context;

        public BlogPostRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            if (blogPost == null) throw new ArgumentNullException(nameof(blogPost));

            // 1. ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ: Проверяем Slug на дубликаты перед вставкой
            bool slugExists = await _context.BlogPosts.AnyAsync(p => p.Slug == blogPost.Slug);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: Статья со слагом '{blogPost.Slug}' уже существует.");
            }

            // 2. ПРОВЕРКА ВНЕШНЕГО КЛЮЧА (Foreign Key): Если категория указана, проверяем её существование
            if (blogPost.CategoryId.HasValue)
            {
                bool categoryExists = await _context.BlogCategories.AnyAsync(c => c.Id == blogPost.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Нарушение Foreign Key: Указанная категория с ID {blogPost.CategoryId.Value} не существует в базе данных.");
                }
            }

            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            // Возвращаем nullable-тип и подгружаем навигационное свойство Category
            return await _context.BlogPosts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списков + подгружаем данные категории
            return await _context.BlogPosts
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            if (blogPost == null) throw new ArgumentNullException(nameof(blogPost));

            // 1. ПРОВЕРКА КОНСТРЕИНТА УНИКАЛЬНОСТИ ПРИ ОБНОВЛЕНИИ (Исключая текущую запись)
            bool slugExists = await _context.BlogPosts.AnyAsync(p => p.Slug == blogPost.Slug && p.Id != blogPost.Id);
            if (slugExists)
            {
                throw new ArgumentException($"Нарушение Unique Constraint: URL-слаг '{blogPost.Slug}' уже занят другой статьей.");
            }

            // 2. ПРОВЕРКА ВНЕШНЕГО КЛЮЧА ПРИ ОБНОВЛЕНИИ
            if (blogPost.CategoryId.HasValue)
            {
                bool categoryExists = await _context.BlogCategories.AnyAsync(c => c.Id == blogPost.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Нарушение Foreign Key при обновлении: Назначаемая категория с ID {blogPost.CategoryId.Value} не найдена.");
                }
            }

            _context.Entry(blogPost).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }
    }
}