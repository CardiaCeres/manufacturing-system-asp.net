namespace ManufacturingSystem.Models
{
    public class Order
    {
        public long Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? MaterialName { get; set; }
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? TotalAmount { get; set; }
        public string? Status { get; set; }
        public string? OrderDate { get; set; }
        public long UserId { get; set; }
        public User? User { get; set; }
        public string Department { get; set; }
    }
}
