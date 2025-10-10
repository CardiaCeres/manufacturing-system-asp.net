using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IOrderService
    {
        // 取得指定使用者的訂單
        Task<List<Order>> GetOrdersByUserIdAsync(long userId);

        // 建立訂單
        Task<Order> CreateOrderAsync(Order order);

        // 根據 Id 取得訂單
        Task<Order?> GetOrderByIdAsync(long id);

        // 更新訂單
        Task<Order> UpdateOrderAsync(long id, Order order);

        // 刪除訂單
        Task DeleteOrderAsync(long id);

        // 管理者取得所有訂單
        Task<List<Order>> GetAllOrdersAsync();

        // 取得同部門使用者的訂單
        Task<List<Order>> GetOrdersByDepartmentAsync(string department);
    }
}
