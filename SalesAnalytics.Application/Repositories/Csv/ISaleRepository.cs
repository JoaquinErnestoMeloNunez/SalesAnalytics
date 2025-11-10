using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories.Csv
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetSalesDataAsync();
    }
}
