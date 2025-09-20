using ManufacturingSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersByUserIdAsync(long userId);
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(long id);
        Task DeleteAsync(long id);
        Task<Order> UpdateAsync(Order order);
    }
}
