namespace SalesAnalytics.Application.Repositories.Base
{
    public interface IFileReaderRepository<TClass> where TClass : class
    {
        Task<IEnumerable<TClass>> ReadFileAsync<T>();
    }
}
