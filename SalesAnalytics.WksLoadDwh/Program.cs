using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Application.Repositories.Db;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Persistence.Repositories.Csv;
using SalesAnalytics.Persistence.Repositories.Db;
using SalesAnalytics.Persistence.Repositories.Db.Context;
using SalesAnalytics.Persistence.Repositories.Dwh;
using SalesAnalytics.Persistence.Repositories.Dwh.Context;

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

            builder.Services.AddDbContext<SaleContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ExternalSalesDb")));

            // Repo que usa el DbContext
            builder.Services.AddScoped<ISaleDbReaderRepository, SaleDbReaderRepository>();

            builder.Services.AddScoped<ISaleRepository, SaleRepository>();

            builder.Services.AddDbContext<DwhSaleAnalitycsContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DataWarehouse")));

            builder.Services.AddScoped<IDwhRepository, DwhRepository>();
            builder.Services.AddScoped<IDwhHandlerService, DwhTransformService>();

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}