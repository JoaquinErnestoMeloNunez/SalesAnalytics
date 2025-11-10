using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;
using System.Globalization;

namespace SalesAnalytics.Persistence.Repositories.Csv
{
    public class CsvOrderReaderRepository : ICsvOrderReaderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _filePath;
        private readonly ILogger<CsvOrderReaderRepository> _logger;

        public CsvOrderReaderRepository(IConfiguration configuration, string? filePath, ILogger<CsvOrderReaderRepository> logger)
        {
            _configuration = configuration;
            _filePath = filePath;
            _logger = logger;
        }

        public async Task<IEnumerable<Order>> ReadFileAsync<T>()
        {
            List<Order> sales = new();
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _logger.LogError("File not found: {FilePath}", _filePath);
                    return (IEnumerable<Order>) sales;
                }

                using var reader = new StreamReader(_filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                await foreach (var record in csv.GetRecordsAsync<Order>())
                {
                    sales.Add(record);
                }
            }
            catch (Exception ex)
            {
                sales = null!;
                _logger.LogError(ex, "Error reading CSV file: {FilePath}", _filePath);
            }
            return (IEnumerable<Order>)sales;
        }
    }
}
