using SalesAnalytics.Application.Repositories;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Persistence.Repositories.Api
{
    public class CustomerApiRepository : ICustomerApiRespository
    {
        public Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
