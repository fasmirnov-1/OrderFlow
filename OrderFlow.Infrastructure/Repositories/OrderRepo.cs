using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Infrastructure.Repositories
{
    // Реализуем целевой интерфейс IOrderRepo
    public class OrderRepo : IOrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            // Возвращаем nullable-тип на случай отсутствия записи
            return await _context.Orders.FindAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Используем AsNoTracking() для оптимизации вывода списков в админ-панели
            // Сортируем по индексу CreatedAt (сначала новые заказы)
            return await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(int status)
        {
            // Быстрая выборка благодаря индексу IX_Orders_Status
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return Enumerable.Empty<Order>();

            // Быстрая выборка по индексу IX_Orders_Email для поиска заказов клиента
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}