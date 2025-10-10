using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public interface IOrderRepository
    {
        // 使用者操作
        Task<List<Order>> GetOrdersByUserIdAsync(long userId);
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(long id);
        Task<Order> UpdateAsync(Order order);
        Task DeleteAsync(long id);

        // 管理者操作
        Task<List<Order>> GetAllAsync(); // 取得所有訂單
        Task<List<Order>> GetByDepartmentAsync(string department); // 同部門訂單
    }
}
