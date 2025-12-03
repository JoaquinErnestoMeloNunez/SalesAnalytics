using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Api.Data.Context;
using SalesAnalytics.Api.Data.Entities;

namespace SalesAnalytics.Persistence.Repositories.Api
{
    public class CustomerApiRepository : ICustomerApiRespository
    {
        private readonly CustomerContext _context;

        public CustomerApiRepository(CustomerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
