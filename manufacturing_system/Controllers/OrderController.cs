using ManufacturingSystem.Models;
using ManufacturingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ManufacturingSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            var user = (User?)HttpContext.Items["User"];
            if (user == null) return Unauthorized("無效使用者");

            var orders = await _orderService.GetOrdersByUserIdAsync(user.Id);
            return Ok(orders);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var user = (User?)HttpContext.Items["User"];
            if (user == null) return Unauthorized("無效使用者");

            order.UserId = user.Id;
            if (string.IsNullOrEmpty(order.ProductName)) return BadRequest("產品名稱為必填");

            var created = await _orderService.CreateOrderAsync(order);
            return Ok(created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateOrder(long id, [FromBody] Order order)
        {
            try
            {
                var updated = await _orderService.UpdateOrderAsync(id, order);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"更新失敗: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"刪除失敗: {ex.Message}");
            }
        }
    }
}
