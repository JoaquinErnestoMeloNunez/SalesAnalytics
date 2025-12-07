using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Dtos;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Domain.Entities.Dwh.Dimensions;
using SalesAnalytics.Domain.Entities.Dwh.Facts;
using SalesAnalytics.Persistence.Repositories.Dwh.Context;

namespace SalesAnalytics.Persistence.Repositories.Dwh
{
    public class DwhRepository : IDwhRepository
    {
        private readonly DwhSaleAnalitycsContext _context;
        private readonly ILogger<DwhRepository> _logger;

        public DwhRepository(DwhSaleAnalitycsContext context, ILogger<DwhRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result> LoadDataAsync(DwhLoadDto data)
        {
            var result = new Result();
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("**Iniciando carga de Dimensiones**");

                    if (data.Customers != null && data.Customers.Any())
                    {
                        var customersEntities = data.Customers.Select(c => new DimCustomers
                        {
                            Customer_Id = c.CustomerID,
                            Customer_Name = c.CustomerName,
                            City = c.City,
                            Country = c.Country
                        }).ToList();

                        await _context.DimCustomers.AddRangeAsync(customersEntities);
                    }

                    if (data.Products != null && data.Products.Any())
                    {
                        var productsEntities = data.Products.Select(p => new DimProducts
                        {
                            Product_Id = p.ProductID,
                            Product_Name = p.ProductName ?? "Sin Nombre",
                            Category = p.Category ?? "Sin Categoria",
                            List_Price = p.ListPrice
                        }).ToList();

                        await _context.DimProducts.AddRangeAsync(productsEntities);
                    }

                    if (data.Dates != null && data.Dates.Any())
                    {
                        await LoadDatesInternalAsync(data.Dates);
                    }
                    
                    await _context.SaveChangesAsync();

                    if (data.Sales != null && data.Sales.Any())
                    {
                        await LoadFactsInternalAsync(data.Sales);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    result.IsSuccess = true;
                    result.Message = $"Carga Exitosa. Clientes: {data.Customers?.Count}, Productos: {data.Products?.Count}";
                    _logger.LogInformation(result.Message);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error en la carga de dimensiones. Se hizo Rollback.");
                    result.IsSuccess = false;
                    result.Message = ex.Message;
                }
            });

            return result;
        }

        public async Task CleanDataWarehouseAsync()
        {
            _logger.LogInformation("***Iniciando limpieza del Data Warehouse***");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Fact].[FactVentas]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Fact].[FactVentas]', RESEED, 0)");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[Dim_Customers]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Dimension].[Dim_Customers]', RESEED, 0)");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[Dim_Products]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Dimension].[Dim_Products]', RESEED, 0)");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[Dim_Date]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Dimension].[Dim_Date]', RESEED, 0)");
                
                await transaction.CommitAsync();

                _logger.LogInformation("Data Warehouse limpio.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error durante la limpieza del DWH.");
                throw;
            }
        }

        private async Task LoadDatesInternalAsync(List<DateDto> dateDtos)
        {
            _logger.LogInformation("Procesando {Count} fechas...", dateDtos.Count);

            var existingDateIds = await _context.DimDates
                .Select(d => d.Date_Id)
                .ToListAsync();

            var existingSet = new HashSet<int>(existingDateIds);
            var datesToInsert = new List<DimDate>();

            foreach (var dto in dateDtos)
            {
                if (!existingSet.Contains(dto.DateId))
                {
                    datesToInsert.Add(new DimDate
                    {
                        Date_Id = dto.DateId,
                        Date = dto.FullDate,
                        Anio = dto.Year,
                        Trimestre = dto.Quarter,
                        Mes = dto.Month,
                        Nombre_Mes = dto.MonthName,
                        Semana = dto.Week,
                        Dia_Mes = dto.DayOfMonth,
                        Dia_Semana = dto.DayOfWeek,
                        Nombre_Dia = dto.DayName
                    });
                    existingSet.Add(dto.DateId);
                }
            }

            if (datesToInsert.Any())
            {   
                await _context.DimDates.AddRangeAsync(datesToInsert);
                _logger.LogInformation("Se detectaron {Count} fechas nuevas.", datesToInsert.Count);
            }
        }

        private async Task LoadFactsInternalAsync(List<FactSalesDto> salesDtos)
        {
            var customers = await _context.DimCustomers.Where(c => c.Customer_Id != null).ToListAsync();
            var products = await _context.DimProducts.ToListAsync();
            var dates = await _context.DimDates.ToListAsync();

            var facts = (from venta in salesDtos
                         join cust in customers on venta.SourceCustomerId equals cust.Customer_Id
                         join prod in products on venta.SourceProductId equals prod.Product_Id
                         let ventaDateId = (venta.OrderDate.Year * 10000) + (venta.OrderDate.Month * 100) + venta.OrderDate.Day
                         join date in dates on ventaDateId equals date.Date_Id

                         select new FactVentas
                         {
                             FK_Customer = cust.Customer_Key,
                             FK_Product = prod.Product_Key,
                             FK_Date = date.Date_Key,
                             Quantity = venta.Quantity,
                             Total_Venta = venta.Total_Venta,
                             Status = venta.Status
                         }).ToList();
            if (facts.Any())
            {
                await _context.FactSales.AddRangeAsync(facts);
            }
        }
    }
}
