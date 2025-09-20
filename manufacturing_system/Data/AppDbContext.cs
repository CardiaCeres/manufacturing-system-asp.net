using ManufacturingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
    }
}
