using SalesAnalytics.Application.Services;

namespace SalesAnalytics.Application.Contracts
{
    public interface IEtlProcessService
    {
        public Task<Result> ExecEtlProcessAsync();
    }
}
