namespace Core.Redis.Helpers;

public interface IDistributedHelper
{
    Task RemoveCache(string cacheGroupKey, string cacheKey,
        CancellationToken cancellationToken);

    Task<TResponse> GetResponse<TResponse>(string cacheKey,
        TResponse response,
        CancellationToken cancellationToken);
    Task<TResponse> GetResponseAndAddToCache<TResponse>(string cacheGroupKey, string cacheKey, TResponse response,
        CancellationToken cancellationToken);

    Task<TResponse> AddToCache<TResponse>(string cacheGroupKey, string cacheKey,
        TResponse response,
        CancellationToken cancellationToken);
}