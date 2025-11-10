using SalesAnalytics.Application.Repositories.Base;
using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories.Csv
{
    public interface ICsvProductReaderRepository : IFileReaderRepository<Product>
    {
    }
}
