using ManufacturingSystem.Models;
using ManufacturingSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<List<Order>> GetOrdersByUserIdAsync(long userId) =>
            await _orderRepository.GetOrdersByUserIdAsync(userId);

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order.Quantity.HasValue && order.Price.HasValue)
                order.TotalAmount = order.Quantity.Value * order.Price.Value;

            order.Status ??= "處理中";
            return await _orderRepository.AddAsync(order);
        }

        public async Task<Order?> GetByIdAsync(long id) =>
            await _orderRepository.GetByIdAsync(id);

        public async Task<Order> UpdateOrderAsync(long id, Order order)
        {
            var existing = await _orderRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("訂單不存在");

            existing.OrderNumber = order.OrderNumber;
            existing.CustomerName = order.CustomerName;
            existing.MaterialName = order.MaterialName;
            existing.ProductName = order.ProductName;
            existing.Quantity = order.Quantity;
            existing.Price = order.Price;
            existing.TotalAmount = order.TotalAmount;
            existing.Status = order.Status;
            existing.OrderDate = order.OrderDate;

            return await _orderRepository.UpdateAsync(existing);
        }

        public async Task DeleteOrderAsync(long id) =>
            await _orderRepository.DeleteAsync(id);
    }
}
