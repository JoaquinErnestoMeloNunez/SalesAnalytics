using SalesAnalytics.Domain.Entities.Csv;

namespace SalesAnalytics.Application.Repositories.Db
{
    public interface ISaleDbReaderRepository
    {
        public Task<IEnumerable<Sale>> GetAllSalesAsync();
    }
}
