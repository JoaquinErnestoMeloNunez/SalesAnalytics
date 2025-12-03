
using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Api.Data.Context;
using SalesAnalytics.Api.Data.Interface;
using SalesAnalytics.Api.Data.Repositories;
using SalesAnalytics.Persistence.Repositories.Api;

namespace SalesAnalytics.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CustomerContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Source"));
            });

            builder.Services.AddDbContext<ProductContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Source"));
            });

            //repositories
            builder.Services.AddScoped<ICustomerApiRespository, CustomerApiRepository>();
            builder.Services.AddScoped<IProductApiRepository, ProductApiRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
