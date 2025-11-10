using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Persistence.Repositories.Csv;

namespace SalesAnalytics.WksLoadDwh
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = builder.Configuration;

            // Rutas del appsettings.json
            string customerPath = configuration.GetValue<string>("CsvFilePaths:Customers")!;
            string productPath = configuration.GetValue<string>("CsvFilePaths:Products")!;
            string orderPath = configuration.GetValue<string>("CsvFilePaths:Orders")!;
            string orderDetailPath = configuration.GetValue<string>("CsvFilePaths:OrderDetails")!;

            // Registro de servicios
            builder.Services.AddScoped<ICsvCustomerReaderRepository>(sp =>
                new CsvCustomerReaderRepository(
                    configuration,
                    customerPath,
                    sp.GetRequiredService<ILogger<CsvCustomerReaderRepository>>()
                ));

            builder.Services.AddScoped<ICsvProductReaderRepository>(sp =>
                new CsvProductReaderRepository(
                    configuration,
                    productPath,
                    sp.GetRequiredService<ILogger<CsvProductReaderRepository>>()
                ));

            builder.Services.AddScoped<ICsvOrderReaderRepository>(sp =>
                new CsvOrderReaderRepository(
                    configuration,
                    orderPath,
                    sp.GetRequiredService<ILogger<CsvOrderReaderRepository>>()
                ));

            builder.Services.AddScoped<ICsvOrderDetailReaderRepository>(sp =>
                new CsvOrderDetailReaderRepository(
                    configuration,
                    orderDetailPath,
                    sp.GetRequiredService<ILogger<CsvOrderDetailReaderRepository>>()
                ));

            builder.Services.AddScoped<ISaleRepository, SaleRepository>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}