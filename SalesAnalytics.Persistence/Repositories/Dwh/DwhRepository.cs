using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Dtos;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Domain.Entities.Dwh.Dimensions;
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

        public async Task<Result> LoadDimsDataAsync(DimDtos dimDtos)
        {
            var result = new Result();
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("Iniciando carga masiva de Dimensiones...");

                    if (dimDtos.Customers != null && dimDtos.Customers.Any())
                    {
                        var customersEntities = dimDtos.Customers.Select(c => new DimCustomers
                        {
                            Customer_Id = c.CustomerID,
                            Customer_Name = c.CustomerName,
                            City = c.City,
                            Country = c.Country
                        }).ToList();

                        await _context.DimCustomers.AddRangeAsync(customersEntities);
                    }

                    if (dimDtos.Products != null && dimDtos.Products.Any())
                    {
                        var productsEntities = dimDtos.Products.Select(p => new DimProducts
                        {
                            Product_Id = p.ProductID,
                            Product_Name = p.ProductName ?? "Sin Nombre",
                            Category = p.Category ?? "Sin Categoria",
                            List_Price = p.ListPrice
                        }).ToList();

                        await _context.DimProducts.AddRangeAsync(productsEntities);
                    }

                    if (dimDtos.Dates != null && dimDtos.Dates.Any())
                    {
                        await LoadDatesInternalAsync(dimDtos.Dates);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    result.IsSuccess = true;
                    result.Message = $"Carga Exitosa. Clientes: {dimDtos.Customers?.Count}, Productos: {dimDtos.Products?.Count}";
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
            _logger.LogInformation("Iniciando limpieza del Data Warehouse...");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Fact].[FactVentas]");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[Dim_Customers]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Dimension].[Dim_Customers]', RESEED, 0)");

                // Borramos Productos
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Dimension].[Dim_Products]");
                await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[Dimension].[Dim_Products]', RESEED, 0)");
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
    }
}
