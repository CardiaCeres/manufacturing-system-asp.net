using ManufacturingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ManufacturingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 將 UserRole enum 存成文字
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>()  // 轉成文字存入資料庫
                .HasMaxLength(20)
                .IsRequired();

            base.OnModelCreating(modelBuilder);

            // TokenExpiry 使用 UTC，並指定 PostgreSQL timestamptz
            var dateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
            );

            var dateTimeComparer = new ValueComparer<DateTime?>(
                (c1, c2) => c1.Equals(c2),
                c => c.HasValue ? c.Value.GetHashCode() : 0,
                c => c.HasValue ? DateTime.SpecifyKind(c.Value, DateTimeKind.Utc) : null
            );

            modelBuilder.Entity<User>()
                .Property(u => u.TokenExpiry)
                .HasColumnType("timestamptz")
                .HasConversion(dateTimeConverter)
                .Metadata.SetValueComparer(dateTimeComparer);
        }
    }
}
