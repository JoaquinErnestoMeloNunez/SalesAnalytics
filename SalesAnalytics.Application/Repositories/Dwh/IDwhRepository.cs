using SalesAnalytics.Application.Dtos;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Domain.Entities.Dwh.Dimensions;

namespace SalesAnalytics.Application.Repositories.Dwh
{
    public interface IDwhRepository
    {
        Task<Result> LoadDataAsync(DwhLoadDto data);
        Task CleanDataWarehouseAsync();
    }
}
