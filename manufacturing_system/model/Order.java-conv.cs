// Auto-converted from Java: Order.java
// NOTE: This is a heuristic conversion to help manual porting. Manual fixes required for compilation and logic.
// Original file: /mnt/data/manufacturing_java_inspect/manufacturing_system/src/main/java/com/manufacturing/model/Order.java

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using jakarta.persistence.Entity
// using jakarta.persistence.GeneratedValue
// using jakarta.persistence.GenerationType
// using jakarta.persistence.Id
// using jakarta.persistence.Table

namespace Manufacturing.Api.ConvertedFromJava.model
{
@Entity
@Table(name = "orders")
public class Order {
 
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private long id;
 
    private string orderNumber;
    private string customerName;
    private string materialName;
    private string productName;
    private int quantity;
    private Double price;         // 單價
    private Double totalAmount;   // 總金額
    private string status;        // 訂單狀態
    private string orderDate;
    
    private long userId; // 直接使用 userId 欄位
 
    // Getter 和 Setter 方法
    public long getId() {
        return id;
    }
    public void setId(long id) {
        this.id = id;
    }
 
    public string getOrderNumber() {
        return orderNumber;
    }
    public void setOrderNumber(string orderNumber) {
        this.orderNumber = orderNumber;
    }
 
    public string getCustomerName() {
        return customerName;
    }
    public void setCustomerName(string customerName) {
        this.customerName = customerName;
    }
 
    public string getMaterialName() {
        return materialName;
    }
    public void setMaterialName(string materialName) {
        this.materialName = materialName;
    }
 
    public string getProductName() {
        return productName;
    }
    public void setProductName(string productName) {
        this.productName = productName;
    }
 
    public int getQuantity() {
        return quantity;
    }
    public void setQuantity(int quantity) {
        this.quantity = quantity;
    }
 
    public Double getPrice() {
        return price;
    }
    public void setPrice(Double price) {
        this.price = price;
    }
 
    public Double getTotalAmount() {
        return totalAmount;
    }
    public void setTotalAmount(Double totalAmount) {
        this.totalAmount = totalAmount;
    }
 
    public string getStatus() {
        return status;
    }
    public void setStatus(string status) {
        this.status = status;
    }
 
    public string getOrderDate() {
        return orderDate;
    }
    public void setOrderDate(string orderDate) {
        this.orderDate = orderDate;
    }
 
    public long getUserId() {
        return userId;
    }
    public void setUserId(long userId) {
        this.userId = userId;
    }
}
}
