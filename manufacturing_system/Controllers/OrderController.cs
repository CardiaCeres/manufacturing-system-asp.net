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

    // 取得訂單列表
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var currentUser = (User?)HttpContext.Items["User"];
        if (currentUser == null) return Unauthorized("無效使用者");

        if (currentUser.Role == UserRole.Manager)
        {
            var orders = await _orderService.GetOrdersByDepartmentAsync(currentUser.Department);
            return Ok(orders);
        }
        else
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(currentUser.Id);
            return Ok(orders);
        }
    }

    // 建立訂單
    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        var currentUser = (User?)HttpContext.Items["User"];
        if (currentUser == null) return Unauthorized("無效使用者");

        if (string.IsNullOrEmpty(order.ProductName))
            return BadRequest("產品名稱為必填");

        // 部門與使用者權限控制
        if (currentUser.Role == UserRole.Manager)
        {
            order.Department = currentUser.Department; // 強制設定部門
        }
        else
        {
            order.UserId = currentUser.Id;
            order.Department = currentUser.Department; // 強制同步部門
        }

        var created = await _orderService.CreateOrderAsync(order);
        return Ok(created);
    }

    // 更新訂單
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateOrder(long id, [FromBody] Order order)
    {
        var currentUser = (User?)HttpContext.Items["User"];
        if (currentUser == null) return Unauthorized("無效使用者");

        var existing = await _orderService.GetOrderByIdAsync(id);
        if (existing == null) return NotFound("訂單不存在");

        // 權限檢查
        if (currentUser.Role == UserRole.Manager && existing.Department != currentUser.Department)
            return Forbid("沒有權限更新此訂單");

        if (currentUser.Role == UserRole.User && existing.UserId != currentUser.Id)
            return Forbid("沒有權限更新此訂單");

        order.Id = id;
        order.Department = existing.Department; // 防止部門亂改
        order.UserId = existing.UserId;

        var updated = await _orderService.UpdateOrderAsync(id, order);
        return Ok(updated);
    }

    // 刪除訂單
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOrder(long id)
    {
        var currentUser = (User?)HttpContext.Items["User"];
        if (currentUser == null) return Unauthorized("無效使用者");

        var existing = await _orderService.GetOrderByIdAsync(id);
        if (existing == null) return NotFound("訂單不存在");

        if (currentUser.Role == UserRole.Manager && existing.Department != currentUser.Department)
            return Forbid("沒有權限刪除此訂單");

        if (currentUser.Role == UserRole.User && existing.UserId != currentUser.Id)
            return Forbid("沒有權限刪除此訂單");

        await _orderService.DeleteOrderAsync(id);
        return NoContent();
           }
        }
    }
}
