using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;
using System.Globalization;

namespace SalesAnalytics.Persistence.Repositories.Csv
{
    public class CsvCustomerReaderRepository : ICsvCustomerReaderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _filePath;
        private readonly ILogger<CsvCustomerReaderRepository> _logger;

        public CsvCustomerReaderRepository(IConfiguration configuration, string? filePath, ILogger<CsvCustomerReaderRepository> logger)
        {
            _configuration = configuration;
            _filePath = filePath;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> ReadFileAsync<T>()
        {
            List<Customer> customers = new();
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _logger.LogError("File not found: {FilePath}", _filePath);
                    return customers;
                }

                using var reader = new StreamReader(_filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                await foreach (var record in csv.GetRecordsAsync<Customer>())
                {
                    customers.Add(record);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV file: {FilePath}", _filePath);
                return Enumerable.Empty<Customer>();
            }
            return customers;
        }
    }
}
