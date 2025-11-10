using SalesAnalytics.Domain.Entities.Api;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Persistence.Repositories.Api
{
    public class ProductApiRepository : IProductApiRepository
    {
        public Task<IEnumerable<Product>> GetProductsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
