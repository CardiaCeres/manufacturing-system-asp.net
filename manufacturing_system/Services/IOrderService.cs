using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// 取得指定使用者的訂單
        /// </summary>
        Task<List<Order>> GetOrdersByUserIdAsync(long userId);

        /// <summary>
        /// 建立訂單
        /// </summary>
        Task<Order> CreateOrderAsync(Order order);

        /// <summary>
        /// 根據 Id 取得訂單
        /// </summary>
        Task<Order?> GetByIdAsync(long id);

        /// <summary>
        /// 更新訂單
        /// </summary>
        Task<Order> UpdateOrderAsync(long id, Order order);

        /// <summary>
        /// 刪除訂單
        /// </summary>
        Task DeleteOrderAsync(long id);

        /// <summary>
        /// 管理者取得所有訂單
        /// </summary>
        Task<List<Order>> GetAllOrdersAsync();

        /// <summary>
        /// 取得同部門使用者的訂單
        /// </summary>
        Task<List<Order>> GetOrdersByDepartmentAsync(string department);
    }
}
