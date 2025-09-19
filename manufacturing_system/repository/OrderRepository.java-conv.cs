// Auto-converted from Java: OrderRepository.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/repository/OrderRepository.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
// using Spring equivalent: org.springframework.data.jpa.repository.JpaRepository
// using com.manufacturing.model.Order

namespace Manufacturing.Api.ConvertedFromJava.repository
{
public interface OrderRepository : JpaRepository<Order, long> {
 List<Order> findByUserIdOrderByOrderNumberAsc(long userId);// 根據 User ID 查詢訂單

 List<Order> findByUserId(long userId);
}
}
