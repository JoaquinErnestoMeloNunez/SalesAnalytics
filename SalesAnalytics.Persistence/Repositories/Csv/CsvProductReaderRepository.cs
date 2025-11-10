using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Base;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;
using System.Globalization;

namespace SalesAnalytics.Persistence.Repositories.Csv
{
    public sealed class CsvProductReaderRepository : ICsvProductReaderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _filePath;
        private readonly ILogger<CsvProductReaderRepository> _logger;

        public CsvProductReaderRepository(IConfiguration configuration, string? filePath, ILogger<CsvProductReaderRepository> logger)
        {
            _configuration = configuration;
            _filePath = filePath;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> ReadFileAsync<T>()
        {
            List<Product> products = new();
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _logger.LogError("File not found: {FilePath}", _filePath);
                    return products;
                }

                using var reader = new StreamReader(_filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                await foreach (var record in csv.GetRecordsAsync<Product>())
                {
                    products.Add(record);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV file: {FilePath}", _filePath);
                return Enumerable.Empty<Product>();
                
            }
            return products;
        }
    }
}
