using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс TagRepo, осуществляющий crud операции над сущностью Tag с учетом имеющихся констрент и ключей.
    public class TagRepo
    {
        private AppDbContext _context;

        public TagRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateTagAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
