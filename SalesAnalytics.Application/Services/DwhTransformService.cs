using Microsoft.Extensions.Logging;
using SalesAnalytics.Application.Dtos;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Domain.Entities.Csv;
using System.Globalization;

namespace SalesAnalytics.Application.Services
{
    public class DwhTransformService : IDwhHandlerService
    {
        private readonly IDwhRepository _dwhRepository;
        private readonly ILogger<DwhTransformService> _logger;

        public DwhTransformService(IDwhRepository dwhRepository, ILogger<DwhTransformService> logger)
        {
            _dwhRepository = dwhRepository;
            _logger = logger;
        }

        public async Task<Result> TransformAndLoadDimensions(
            List<Customer> rawCustomers, 
            List<Product> rawProducts, 
            List<Sale> sales)
        {
            _logger.LogInformation("Handler: Iniciando Transformación de datos...");

            var dimDtos = new DwhLoadDto();

            dimDtos.Customers = rawCustomers.Select(c => new CustomerDto
            {
                CustomerID = c.CustomerID,
                CustomerName = $"{c.FirstName} {c.LastName}".Trim(),
                City = c.City ?? "Desconocido",
                Country = c.Country ?? "Desconocido"
            }).ToList();

            dimDtos.Products = rawProducts.Select(p => new ProductDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName ?? "Desconocido",
                Category = p.Category ?? "Sin Categoría",
                ListPrice = p.Price
            }).ToList();

            _logger.LogInformation("Handler: Calculando Dim_Date");

            var uniqueDates = sales
                .Select(s => s.OrderDate)
                .Distinct()
                .ToList();

            var culture = new CultureInfo("es-ES");

            dimDtos.Dates = uniqueDates.Select(d =>
            {
                DateTime dt = d.ToDateTime(TimeOnly.MinValue);

                return new DateDto
                {
                    DateId = (dt.Year * 10000) + (dt.Month * 100) + dt.Day,
                    FullDate = dt,
                    Year = dt.Year,
                    Month = dt.Month,
                    DayOfMonth = dt.Day,
                    DayOfWeek = (int)dt.DayOfWeek,
                    Quarter = ((dt.Month - 1) / 3) + 1,
                    Week = culture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    MonthName = culture.TextInfo.ToTitleCase(dt.ToString("MMMM", culture)),
                    DayName = culture.TextInfo.ToTitleCase(dt.ToString("dddd", culture))
                };
            }).ToList();

            dimDtos.Sales = sales.Select(s => new FactSalesDto
            {
                SourceCustomerId = s.CustomerID,
                SourceProductId = s.ProductID,
                OrderDate = s.OrderDate.ToDateTime(TimeOnly.MinValue),
                Quantity = s.Quantity,
                Total_Venta = s.TotalPrice,
                Status = s.Status ?? "Desconocido"
            }).ToList();

            _logger.LogInformation("Handler: Datos transformados. Enviando al Repositorio...");

            return await _dwhRepository.LoadDataAsync(dimDtos);
        }
    }
}
