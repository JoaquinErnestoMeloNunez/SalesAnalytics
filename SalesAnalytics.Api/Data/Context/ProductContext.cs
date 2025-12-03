using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Api.Data.Entities;

namespace SalesAnalytics.Api.Data.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.ProductID);
                entity.Property(e => e.ProductID).HasColumnName("product_id");
                entity.Property(e => e.ProductName).HasColumnName("product_name");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Stock).HasColumnName("stock");
            });
        }
    }
}
