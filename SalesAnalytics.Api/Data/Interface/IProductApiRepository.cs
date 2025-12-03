using SalesAnalytics.Api.Data.Entities;

namespace SalesAnalytics.Api.Data.Interface
{
    public interface IProductApiRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
    }
}
