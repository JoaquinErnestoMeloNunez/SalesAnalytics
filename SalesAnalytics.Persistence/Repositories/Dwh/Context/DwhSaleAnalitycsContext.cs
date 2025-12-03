using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Domain.Entities.Dwh.Dimensions;

namespace SalesAnalytics.Persistence.Repositories.Dwh.Context
{
    public class DwhSaleAnalitycsContext : DbContext
    {
        public DwhSaleAnalitycsContext(DbContextOptions<DwhSaleAnalitycsContext> options) : base(options)
        {
        }

        public DbSet<DimCustomers> DimCustomers { get; set; }
        public DbSet<DimProducts> DimProducts { get; set; }
        public DbSet<DimDate> DimDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DimCustomers>(entity =>
            {
                entity.ToTable("Dim_Customers", "Dimension");
                entity.HasKey(e => e.Customer_Key);

                entity.Property(e => e.Customer_Key).HasColumnName("Customer_Key");
                entity.Property(e => e.Customer_Id).HasColumnName("Customer_Id");
                entity.Property(e => e.Customer_Name).HasColumnName("Customer_Name").HasMaxLength(200);
                entity.Property(e => e.City).HasColumnName("City").HasMaxLength(200);
                entity.Property(e => e.Country).HasColumnName("Country").HasMaxLength(200);
            });

            modelBuilder.Entity<DimProducts>(entity =>
            {
                entity.ToTable("Dim_Products", "Dimension");
                entity.HasKey(e => e.Product_Key);

                entity.Property(e => e.Product_Key).HasColumnName("Product_Key");
                entity.Property(e => e.Product_Id).HasColumnName("Product_Id");
                entity.Property(e => e.Product_Name).HasColumnName("Product_Name").HasMaxLength(100);
                entity.Property(e => e.Category).HasColumnName("Category").HasMaxLength(100);
                entity.Property(e => e.List_Price).HasColumnName("List_Price").HasColumnType("money");
            });

            modelBuilder.Entity<DimDate>(entity =>
            {
                entity.ToTable("Dim_Date", "Dimension");
                entity.HasKey(e => e.Date_Key);

                entity.Property(e => e.Date_Key).HasColumnName("Date_Key");
                entity.Property(e => e.Date_Id).HasColumnName("Date_Id");
                entity.Property(e => e.Date).HasColumnName("Date").HasColumnType("date");
                entity.Property(e => e.Anio).HasColumnName("Anio");
                entity.Property(e => e.Trimestre).HasColumnName("Trimestre");
                entity.Property(e => e.Mes).HasColumnName("Mes");
                entity.Property(e => e.Nombre_Mes).HasColumnName("Nombre_Mes").HasMaxLength(50);
                entity.Property(e => e.Semana).HasColumnName("Semana");
                entity.Property(e => e.Dia_Mes).HasColumnName("Dia_Mes");
                entity.Property(e => e.Dia_Semana).HasColumnName("Dia_Semana");
                entity.Property(e => e.Nombre_Dia).HasColumnName("Nombre_Dia").HasMaxLength(50);
            });
        }
    }
}