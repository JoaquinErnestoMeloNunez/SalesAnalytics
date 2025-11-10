using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Persistence.Repositories.Csv
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ICsvOrderReaderRepository _orderReader;
        private readonly ICsvOrderDetailReaderRepository _orderDetailReader;
        private readonly ILogger<SaleRepository> _logger;

        public SaleRepository(
            ICsvOrderReaderRepository orderReader,
            ICsvOrderDetailReaderRepository orderDetailReader,
            ILogger<SaleRepository> logger)
        {
            _orderReader = orderReader;
            _orderDetailReader = orderDetailReader;
            _logger = logger;
        }

        public async Task<IEnumerable<Sale>> GetSalesDataAsync()
        {
            try
            {
                //
                // FASE DE EXTRACCIÓN (E)
                //
                _logger.LogInformation("Starting Order Extractor...");
                // Tu diseño de repo ignora el parámetro, así que pasamos string.Empty
                var ordersTask = _orderReader.ReadFileAsync<Order>();

                _logger.LogInformation("Iniciando extracción de OrderDetails...");
                var orderDetailsTask = _orderDetailReader.ReadFileAsync<OrderDetail>();

                // Ejecución paralela de ambas tareas
                await Task.WhenAll(ordersTask, orderDetailsTask);

                var orders = (await ordersTask).ToList();
                var orderDetails = (await orderDetailsTask).ToList();

                _logger.LogInformation("Extracción completa. Orders: {OrderCount}, OrderDetails: {DetailCount}", orders.Count, orderDetails.Count);

                //
                // FASE DE TRANSFORMACIÓN (T)
                //
                _logger.LogInformation("Iniciando transformación (join) de datos de ventas...");

                // LINQ para hacer el join en memoria
                var salesData = from o in orders
                                join od in orderDetails on o.OrderID equals od.OrderID
                                select new Sale
                                {
                                    // Datos de Order
                                    OrderID = o.OrderID,
                                    CustomerID = o.CustomerID,
                                    OrderDate = o.OrderDate,
                                    Status = o.Status,

                                    // Datos de OrderDetail
                                    ProductID = od.ProductID,
                                    Quantity = od.Quantity,
                                    TotalPrice = od.TotalPrice
                                };

                var result = salesData.ToList();
                _logger.LogInformation("Transformación completa. {SaleCount} registros de ventas combinados.", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de E/T de SalesRepository.");
                return Enumerable.Empty<Sale>();
            }
        }
    }
}
