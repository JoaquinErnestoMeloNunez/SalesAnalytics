using SalesAnalytics.Domain.Entities.Csv;
using SalesAnalytics.Application.Services;

namespace SalesAnalytics.Application.Repositories.Dwh
{
    public interface IDwhHandlerService
    {
        Task<Result> TransformAndLoadDimensions(List<Customer> customers, List<Product> products, List<Sale> orders);
    }
}
