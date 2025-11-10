using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories
{
    public interface ICustomerApiRespository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
    }
}
