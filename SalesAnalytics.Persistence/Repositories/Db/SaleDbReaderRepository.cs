using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Db;
using SalesAnalytics.Domain.Entities.Csv;
using SalesAnalytics.Persistence.Repositories.Db.Context;

namespace SalesAnalytics.Persistence.Repositories.Db
{
    public class SaleDbReaderRepository : ISaleDbReaderRepository
    {
        private readonly SaleContext _context;
        private readonly ILogger<SaleDbReaderRepository> _logger;

        public SaleDbReaderRepository(SaleContext context, ILogger<SaleDbReaderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            _logger.LogInformation("Consultando historial usando EF Core...");

            try
            {
                var query = from o in _context.Orders
                            join od in _context.OrderDetails on o.OrderID equals od.OrderID
                            select new Sale
                            {
                                OrderID = o.OrderID,
                                CustomerID = o.CustomerID,
                                OrderDate = o.OrderDate,
                                Status = o.Status,

                                ProductID = od.ProductID,
                                Quantity = od.Quantity,
                                TotalPrice = od.TotalPrice
                            };

                var sales = await query.AsNoTracking().ToListAsync();

                _logger.LogInformation("Extracción EF Core finalizada. {Count} registros.", sales.Count);
                return sales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer ventas históricas con EF Core.");
                return Enumerable.Empty<Sale>();
            }
        }
    }
}
