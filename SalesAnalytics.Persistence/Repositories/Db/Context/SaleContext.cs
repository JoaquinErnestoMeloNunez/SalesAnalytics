using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Persistence.Repositories.Db.Context
{
    public class SaleContext : DbContext
    {
        public SaleContext(DbContextOptions<SaleContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.HasKey(e => e.OrderID);
                entity.Property(e => e.OrderID).HasColumnName("order_id");
                entity.Property(e => e.CustomerID).HasColumnName("customer_id");
                entity.Property(e => e.OrderDate).HasColumnName("order_date");
                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("order_detail");
                entity.HasKey(e => new { e.OrderID, e.ProductID });

                entity.Property(e => e.OrderID).HasColumnName("order_id");
                entity.Property(e => e.ProductID).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.TotalPrice).HasColumnName("subtotal");
            });
        }
    }
}
