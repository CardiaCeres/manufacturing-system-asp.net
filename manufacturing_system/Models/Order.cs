namespace ManufacturingSystem.Models
{
    public class Order
    {
        public long Id { get; set; } // BIGINT PRIMARY KEY IDENTITY
        public string? OrderNumber { get; set; } // VARCHAR(255)
        public string? CustomerName { get; set; } // VARCHAR(255)
        public string? MaterialName { get; set; } // VARCHAR(255)
        public string? ProductName { get; set; } // VARCHAR(255)
        public int? Quantity { get; set; } // INTEGER
        public double? Price { get; set; } // DOUBLE PRECISION
        public double? TotalAmount { get; set; } // DOUBLE PRECISION
        public string? Status { get; set; } // VARCHAR(255)
        public string? OrderDate { get; set; } // VARCHAR(255)
        public long UserId { get; set; } // BIGINT
        public string? Department { get; set; } // VARCHAR(255)
        public User? User { get; set; }
    }
}
