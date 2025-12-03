using SalesAnalytics.Api.Data.Entities;

namespace SalesAnalytics.Persistence.Repositories.Api
{
    public interface ICustomerApiRespository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
    }
}