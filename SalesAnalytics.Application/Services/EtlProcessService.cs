using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Contracts;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Application.Repositories.Db;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Services
{
    public class EtlProcessService : IEtlProcessService
    {
        private readonly ILogger<EtlProcessService> _logger;
        private readonly IDwhRepository _dwhRepository;
        private readonly ISaleRepository _salesRepo;
        private readonly ICsvCustomerReaderRepository _customerRepo;
        private readonly ICsvProductReaderRepository _productRepo;
        private readonly ISaleDbReaderRepository _sqlRepo;
        private readonly IDwhHandlerService _dimHandler;

        public EtlProcessService(
            ILogger<EtlProcessService> logger,
            IDwhRepository dwhRepository,
            ISaleRepository salesRepo,
            ICsvCustomerReaderRepository customerRepo,
            ICsvProductReaderRepository productRepo,
            ISaleDbReaderRepository sqlRepo,
            IDwhHandlerService dimHandler)
        {
            _logger = logger;
            _dwhRepository = dwhRepository;
            _salesRepo = salesRepo;
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _sqlRepo = sqlRepo;
            _dimHandler = dimHandler;
        }

        public async Task<Result> ExecEtlProcessAsync()
        {
            _logger.LogInformation(">>>-- INICIANDO PROCESO ETL --<<<");

            await _dwhRepository.CleanDataWarehouseAsync();

            _logger.LogInformation("Fase de Extraccion");

            var rep1 = _salesRepo.GetSalesDataAsync();
            var rep2 = _sqlRepo.GetAllSalesAsync();
            var rep3 = _customerRepo.ReadFileAsync<Customer>();
            var rep4 = _productRepo.ReadFileAsync<Product>();

            await Task.WhenAll(rep1, rep2, rep3, rep4);

            var csvSales = (await rep1).ToList();
            var sqlSales = (await rep2).ToList();
            var customers = (await rep3).ToList();
            var products = (await rep4).ToList();

            var allSales = csvSales.Concat(sqlSales).ToList();

            _logger.LogInformation($"Datos extraidos: Clientes={customers.Count}, Productos={products.Count}, Ventas={allSales.Count}");
            _logger.LogInformation("Fase de Transformacion y Carga");

            var result = await _dimHandler.TransformAndLoadDimensions(customers, products, allSales);

            if(result.IsSuccess)
                _logger.LogInformation($"Proceso completado con exito: {result.Message}");
            else
                _logger.LogError($"ERROR: {result.Message}");

            return result;
        }
    }
}
