using SalesAnalytics.Application.Repositories.Base;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories.Csv
{
    public interface ICsvOrderDetailReaderRepository : IFileReaderRepository<OrderDetail>
    {
    }
}
