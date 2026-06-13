using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать кдасс, который будет выполнять crud операции для таблицы AspNetUserToken с учетом кнтрент и ключей. Он должен реализовывать интерфейс IDisposible
    public class AspNetUserTokenRepo
    {
        private AppDbContext _context;
        public AspNetUserTokenRepo(AppDbContext context)
        {
            _context = context;
        }
        public void Create(AspNetUserToken token)
        {
            _context.AspNetUserTokens.Add(token);
            _context.SaveChanges();
        }
        public AspNetUserToken Read(string userId, string loginProvider, string name)
        {
            return _context.AspNetUserTokens.FirstOrDefault(t => t.UserId == userId && t.LoginProvider == loginProvider && t.Name == name);
        }
        public void Update(AspNetUserToken token)
        {
            _context.AspNetUserTokens.Update(token);
            _context.SaveChanges();
        }
        public void Delete(string userId, string loginProvider, string name)
        {
            var token = Read(userId, loginProvider, name);
            if (token != null)
            {
                _context.AspNetUserTokens.Remove(token);
                _context.SaveChanges();
            }
        }
    }
}
