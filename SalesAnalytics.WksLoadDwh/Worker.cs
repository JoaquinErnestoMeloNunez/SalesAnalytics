using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Application.Repositories.Db;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Domain.Entities.Csv;

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
                    var dwhRepository = scope.ServiceProvider.GetRequiredService<IDwhRepository>();
                    await dwhRepository.CleanDataWarehouseAsync();

                    _logger.LogInformation("Iniciando Fase de Extracción");

                    var salesRepository = scope.ServiceProvider.GetRequiredService<ISaleRepository>();
                    var customerRepository = scope.ServiceProvider.GetRequiredService<ICsvCustomerReaderRepository>();
                    var productRepository = scope.ServiceProvider.GetRequiredService<ICsvProductReaderRepository>();
                    // --- El repo de SQL ---
                    var sqlRepository = scope.ServiceProvider.GetRequiredService<ISaleDbReaderRepository>();

                    _logger.LogInformation("Extrayendo y transformando Ventas (Orders y OrderDetails)");
                    var salesTask = salesRepository.GetSalesDataAsync();

                    _logger.LogInformation("Extrayendo Ventas Históricas SQL...");
                    // --- Iniciar extracción SQL ---
                    var salesSqlTask = sqlRepository.GetAllSalesAsync();

                    _logger.LogInformation("Extrayendo Customers");
                    var customerTask = customerRepository.ReadFileAsync<Customer>();

                    _logger.LogInformation("Extrayendo Products");
                    var productTask = productRepository.ReadFileAsync<Product>();

                    await Task.WhenAll(salesTask, salesSqlTask, customerTask, productTask);

                    var salesData = (await salesTask).ToList();
                    var salesSqlData = (await salesSqlTask).ToList();
                    var customerData = (await customerTask).ToList();
                    var productData = (await productTask).ToList();

                    var allSalesData = salesData.Concat(salesSqlData).ToList();

                    _logger.LogInformation("****** Resumen de Extract ******");
                    _logger.LogInformation("Registros de Ventas combinados: {Count}", salesData.Count);
                    _logger.LogInformation("Registros de Clientes extraídos: {Count}", customerData.Count);
                    _logger.LogInformation("Registros de Productos extraídos: {Count}", productData.Count);
                    _logger.LogInformation("*************************************");

                    var dimHandler = scope.ServiceProvider.GetRequiredService<IDwhHandlerService>();

                    var customerDataLoading = (await customerTask).ToList();
                    var productDataLoading = (await productTask).ToList();
                    _logger.LogInformation(">>> Iniciando Carga de Dimensiones al DWH <<<");

                    var loadResult = await dimHandler.TransformAndLoadDimensions(customerDataLoading, productDataLoading, allSalesData);

                    if (loadResult.IsSuccess)
                    {
                        _logger.LogInformation($"EXITO: {loadResult.Message}");
                    }
                    else
                    {
                        _logger.LogError($"ERROR: {loadResult.Message}");
                    }

                    _logger.LogInformation(">>> Fin del Proceso ETL <<<");
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
