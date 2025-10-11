using ManufacturingSystem.Data;
using ManufacturingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) => _context = context;

        // 使用者 CRUD
        public async Task<List<Order>> GetOrdersByUserIdAsync(long userId) =>
            await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Where(o => o.UserId == userId)
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();

        public async Task<Order> AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(long id) =>
            await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteAsync(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        // 管理者功能
        public async Task<List<Order>> GetAllAsync() =>
            await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();

        public async Task<List<Order>> GetByDepartmentAsync(string department) =>
            await _context.Orders
                .AsNoTracking()
                .Include(o => o.User)
                .Where(o => o.Department == department) // 直接使用 Order.Department
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();
    }
}
