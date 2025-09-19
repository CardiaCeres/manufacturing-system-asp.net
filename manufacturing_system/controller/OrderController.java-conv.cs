// Auto-converted from Java: OrderController.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/controller/OrderController.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.http.ResponseEntity
// using Spring equivalent: org.springframework.web.bind.annotation.CrossOrigin
// using Spring equivalent: org.springframework.web.bind.annotation.DeleteMapping
// using Spring equivalent: org.springframework.web.bind.annotation.GetMapping
// using Spring equivalent: org.springframework.web.bind.annotation.PathVariable
// using Spring equivalent: org.springframework.web.bind.annotation.PostMapping
// using Spring equivalent: org.springframework.web.bind.annotation.PutMapping
// using Spring equivalent: org.springframework.web.bind.annotation.RequestBody
// using Spring equivalent: org.springframework.web.bind.annotation.RequestMapping
// using Spring equivalent: org.springframework.web.bind.annotation.RestController
// using com.manufacturing.model.Order
// using com.manufacturing.model.User
// using com.manufacturing.security.JwtUtil
// using com.manufacturing.service.OrderService
// using com.manufacturing.service.UserService
// using Servlet equivalent: jakarta.servlet.http.HttpServletRequest

namespace Manufacturing.Api.ConvertedFromJava.controller
{
@CrossOrigin(origins = "frontendUrl")
[ApiController]
[Route("/api/orders")]
public class OrderController {

    // Autowired -> use constructor injection
    private OrderService orderService;

    // Autowired -> use constructor injection
    private UserService userService;

    // 從 JWT 抽出使用者並取得訂單
    [HttpGet("/my")]
    public ResponseEntity<?> getMyOrders(HttpServletRequest request) {
        try {
            string token = extractTokenFromRequest(request);
            string username = JwtUtil.extractUsername(token);
            User user = userService.getUserByUsername(username);
            if (user == null) {
                return ResponseEntity.status(401).body("無效使用者");
            }

            List<Order> orders = orderService.getOrdersByUserId(user.getId());
            return ResponseEntity.ok(orders);
        } catch (Exception e) {
            return ResponseEntity.internalServerError().body("伺服器錯誤：" + e.getMessage());
        }
    }

    [HttpPost("/create")]
    public ResponseEntity<?> createOrder([FromBody] Order order, HttpServletRequest request) {
        try {
            string token = extractTokenFromRequest(request);
            string username = JwtUtil.extractUsername(token);
            User user = userService.getUserByUsername(username);
            if (user == null) {
                return ResponseEntity.status(401).body("無效使用者");
            }

            // 設定使用者 ID
            order.setUserId(user.getId());

            // 驗證必要欄位
            if (order.getProductName() == null || order.getProductName().isEmpty()) {
                return ResponseEntity.badRequest().body("產品名稱為必填");
            }

            // 計算總金額
            if (order.getQuantity() != null && order.getPrice() != null) {
                order.setTotalAmount(order.getQuantity() * order.getPrice());
            }

            // 預設狀態
            if (order.getStatus() == null || order.getStatus().isEmpty()) {
                order.setStatus("處理中");
            }

            Order createdOrder = orderService.createOrder(order);
            return ResponseEntity.ok(createdOrder);

        } catch (Exception e) {
            return ResponseEntity.internalServerError().body("建立訂單失敗：" + e.getMessage());
        }
    }

    [HttpDelete("/delete/{id}")]
    public ResponseEntity<?> deleteOrder([FromRoute] long id) {
        try {
            orderService.deleteOrder(id);
            return ResponseEntity.noContent().build();
        } catch (Exception e) {
            return ResponseEntity.internalServerError().body("刪除失敗：" + e.getMessage());
        }
    }

    [HttpPut("/update/{id}")]
    public ResponseEntity<?> updateOrder([FromRoute] long id, [FromBody] Order order) {
        try {
            Order updatedOrder = orderService.updateOrder(id, order);
            return ResponseEntity.ok(updatedOrder);
        } catch (Exception e) {
            return ResponseEntity.internalServerError().body("更新失敗：" + e.getMessage());
        }
    }

    private string extractTokenFromRequest(HttpServletRequest request) {
        string bearerToken = request.getHeader("Authorization");
        if (bearerToken != null && bearerToken.startsWith("Bearer ")) {
            return bearerToken.substring(7);
        }
        return null;
    }
}
}
