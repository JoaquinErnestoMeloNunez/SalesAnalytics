using SalesAnalytics.Application.Contracts;
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
                    var etlService = scope.ServiceProvider.GetRequiredService<IEtlProcessService>();
                    await etlService.ExecEtlProcessAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "El Worker falló al intentar ejecutar el proceso ETL");
            }
            finally
            {
                _logger.LogInformation("Worker finalizado en: {time}", DateTimeOffset.Now);
            }
        }
    }
}
