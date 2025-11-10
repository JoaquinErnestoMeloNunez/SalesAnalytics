using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Domain.Entities.Api
{
    public interface IProductApiRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
    }
}
