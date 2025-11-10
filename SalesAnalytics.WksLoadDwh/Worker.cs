using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;
using SalesAnalytics.Persistence.Repositories.Csv;

namespace SalesAnalytics.WksLoadDwh
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado en: {time}", DateTimeOffset.Now);

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    _logger.LogInformation("Iniciando Fase de Extracción");

                    var salesRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();
                    var customerRepository = scope.ServiceProvider.GetRequiredService<ICsvCustomerReaderRepository>();
                    var productRepository = scope.ServiceProvider.GetRequiredService<ICsvProductReaderRepository>();

                    _logger.LogInformation("Extrayendo y transformando Ventas (Orders y OrderDetails)");
                    var salesTask = salesRepository.GetSalesDataAsync();

                    _logger.LogInformation("Extrayendo Customers");
                    var customerTask = customerRepository.ReadFileAsync<Customer>();

                    _logger.LogInformation("Extrayendo Products");
                    var productTask = productRepository.ReadFileAsync<Product>();

                    await Task.WhenAll(salesTask, customerTask, productTask);

                    var salesData = (await salesTask).ToList();
                    var customerData = (await customerTask).ToList();
                    var productData = (await productTask).ToList();

                    _logger.LogInformation("****** Resumen de Extract ******");
                    _logger.LogInformation("Registros de Ventas combinados: {Count}", salesData.Count);
                    _logger.LogInformation("Registros de Clientes extraídos: {Count}", customerData.Count);
                    _logger.LogInformation("Registros de Productos extraídos: {Count}", productData.Count);
                    _logger.LogInformation("*************************************");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "El proceso E fallo.");
            }
            finally
            {
                _logger.LogInformation("Worker finalizado en: {time}", DateTimeOffset.Now);
            }
        }
    }
}
