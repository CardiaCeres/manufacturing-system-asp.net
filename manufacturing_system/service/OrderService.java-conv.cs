// Auto-converted from Java: OrderService.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/service/OrderService.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.Generic;
// using Spring equivalent: org.springframework.beans.factory.annotation.Autowired
// using Spring equivalent: org.springframework.stereotype.Service
// using com.manufacturing.model.Order
// using com.manufacturing.repository.OrderRepository

namespace Manufacturing.Api.ConvertedFromJava.service
{
[ServiceDescriptor] // Map Service to DI registration
public class OrderService {

    // Autowired -> use constructor injection
    private OrderRepository orderRepository;

    public List<Order> getOrdersByUserId(long userId) {
        return orderRepository.findByUserIdOrderByOrderNumberAsc(userId);
    }

    public Order createOrder(Order order) {
        return orderRepository.save(order);
    }

    public void deleteOrder(long id) {
        orderRepository.deleteById(id);
    }

    public Order updateOrder(long id, Order order) {
        Order> existingOrderOpt = orderRepository.findById(id);
        if (existingOrderOpt.isPresent()) {
            Order existingOrder = existingOrderOpt.get();
            existingOrder.setOrderNumber(order.getOrderNumber());
            existingOrder.setCustomerName(order.getCustomerName());
            existingOrder.setMaterialName(order.getMaterialName());
            existingOrder.setProductName(order.getProductName());
            existingOrder.setQuantity(order.getQuantity());
            existingOrder.setPrice(order.getPrice());
            existingOrder.setTotalAmount(order.getTotalAmount());
            existingOrder.setStatus(order.getStatus());
            existingOrder.setOrderDate(order.getOrderDate());
 
            return orderRepository.save(existingOrder);

            
        } else {
            throw new RuntimeException("訂單未找到，無法更新");
        }
    }
}
}
