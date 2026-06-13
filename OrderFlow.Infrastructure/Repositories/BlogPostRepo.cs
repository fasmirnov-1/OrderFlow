using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать классБ выполняющий crud операции для сущности BlogPost, с учетом действующих контрейнов и ключей.
    public class BlogPostRepo
    {
        private AppDbContext _context;

        public BlogPostRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost> GetByIdAsync(int id)
        {
            return await _context.BlogPosts.FindAsync(id);
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts.ToListAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }
    }
}
