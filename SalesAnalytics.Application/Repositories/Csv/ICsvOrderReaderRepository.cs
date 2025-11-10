using SalesAnalytics.Application.Repositories.Base;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories.Csv
{
    public interface ICsvOrderReaderRepository : IFileReaderRepository<Order>
    {
    }
}
