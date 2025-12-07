using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Application.Contracts;
using SalesAnalytics.Application.Repositories.Csv;
using SalesAnalytics.Application.Repositories.Db;
using SalesAnalytics.Application.Repositories.Dwh;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Persistence.Repositories.Csv;
using SalesAnalytics.Persistence.Repositories.Db;
using SalesAnalytics.Persistence.Repositories.Db.Context;
using SalesAnalytics.Persistence.Repositories.Dwh;
using SalesAnalytics.Persistence.Repositories.Dwh.Context;

namespace SalesAnalytics.WksLoadDwh.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEtlInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Rutas de los CSV
            string customerPath = configuration.GetValue<string>("CsvFilePaths:Customers")!;
            string productPath = configuration.GetValue<string>("CsvFilePaths:Products")!;
            string orderPath = configuration.GetValue<string>("CsvFilePaths:Orders")!;
            string orderDetailPath = configuration.GetValue<string>("CsvFilePaths:OrderDetails")!;

            // Repos CSV
            services.AddScoped<ICsvCustomerReaderRepository>(sp =>
                new CsvCustomerReaderRepository(
                    configuration,
                    customerPath,
                    sp.GetRequiredService<ILogger<CsvCustomerReaderRepository>>()
                ));

            services.AddScoped<ICsvProductReaderRepository>(sp =>
                new CsvProductReaderRepository(
                    configuration,
                    productPath,
                    sp.GetRequiredService<ILogger<CsvProductReaderRepository>>()
                ));

            services.AddScoped<ICsvOrderReaderRepository>(sp =>
                new CsvOrderReaderRepository(
                    configuration,
                    orderPath,
                    sp.GetRequiredService<ILogger<CsvOrderReaderRepository>>()
                ));

            services.AddScoped<ICsvOrderDetailReaderRepository>(sp =>
                new CsvOrderDetailReaderRepository(
                    configuration,
                    orderDetailPath,
                    sp.GetRequiredService<ILogger<CsvOrderDetailReaderRepository>>()
                ));

            // Db contexts
            services.AddDbContext<SaleContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ExternalSalesDb")));

            services.AddDbContext<DwhSaleAnalitycsContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DataWarehouse")));

            // Repos y services
            services.AddScoped<ISaleDbReaderRepository, SaleDbReaderRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddScoped<IDwhRepository, DwhRepository>();
            services.AddScoped<IDwhHandlerService, DwhTransformService>();

            services.AddScoped<IEtlProcessService, EtlProcessService>();

            return services;
        }
    }
}
