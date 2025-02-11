namespace Core.Redis.MediaR;

public interface ICacheRemovingRequest
{
    string? CacheKey { get; }
    bool BypassCache { get; }
    string? CacheGroupKey { get; }
}