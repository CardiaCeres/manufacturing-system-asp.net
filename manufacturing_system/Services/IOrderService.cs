using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersByUserIdAsync(long userId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetByIdAsync(long id);
        Task<Order> UpdateOrderAsync(long id, Order order);
        Task DeleteOrderAsync(long id);
    }
}
