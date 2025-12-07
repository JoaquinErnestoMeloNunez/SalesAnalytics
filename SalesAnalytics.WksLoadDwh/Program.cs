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
using SalesAnalytics.WksLoadDwh.Extensions;

namespace SalesAnalytics.WksLoadDwh
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddEtlInfrastructure(builder.Configuration);

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}