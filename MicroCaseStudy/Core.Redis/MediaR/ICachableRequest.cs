namespace Core.Redis.MediaR;

public interface ICachableRequest
{
    public string CacheKey { get; }
    public bool BypassCache { get; }
    public string? CacheGroupKey { get; }
    public TimeSpan? SlidingExpiration { get; }
}