using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Domain.Entities.Csv;
using System.Globalization;

namespace SalesAnalytics.Persistence.Repositories.Csv
{
    public class CsvOrderDetailReaderRepository : ICsvOrderDetailReaderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _filePath;
        private readonly ILogger<CsvOrderDetailReaderRepository> _logger;

        public CsvOrderDetailReaderRepository(IConfiguration configuration, string? filePath, ILogger<CsvOrderDetailReaderRepository> logger)
        {
            _configuration = configuration;
            _filePath = filePath;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDetail>> ReadFileAsync<T>()
        {
            List<OrderDetail> records = new();
            try
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _logger.LogError("File not found: {FilePath}", _filePath);
                    return records;
                }

                using var reader = new StreamReader(_filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                await foreach (var record in csv.GetRecordsAsync<OrderDetail>())
                {
                    records.Add(record);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading CSV file: {FilePath}", _filePath);
                return Enumerable.Empty<OrderDetail>();
            }
            return records;
        }
    }
}
