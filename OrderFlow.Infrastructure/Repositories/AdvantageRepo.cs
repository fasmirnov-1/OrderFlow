using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Repositories
{
    //Создать класс AdvantageRepo, который будет реализовывать интерфейс IAdvantageRepo. Выполнять операции CRUD над сущностью Advantage с учетом имеющихся констрент и ключей. Класс должен реализовыать интерфейс IDisposable для правильного управления ресурсами базы данных.
    public class AdvantageRepo
    {
        private AppDbContext _context;
        public AdvantageRepo(AppDbContext context)
        {
            _context = context;
        }
        public void Create(Advantage advantage)
        {
            _context.Advantages.Add(advantage);
            _context.SaveChanges();
        }
        public Advantage Read(int id)
        {
            return _context.Advantages.Find(id);
        }
        public void Update(Advantage advantage)
        {
            _context.Advantages.Update(advantage);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var advantage = _context.Advantages.Find(id);
            if (advantage != null)
            {
                _context.Advantages.Remove(advantage);
                _context.SaveChanges();
            }
        }
    }
}
