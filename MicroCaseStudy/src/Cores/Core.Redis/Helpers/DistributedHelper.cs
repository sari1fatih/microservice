using System.Text;
using System.Text.Json;
using Core.Redis.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Redis.Helpers;

public class DistributedHelper : IDistributedHelper
{
    private readonly IDistributedCache _cache;
    private readonly CacheSettings _cacheSettings;
    private readonly ILogger<DistributedHelper> _logger;

    public DistributedHelper(ILogger<DistributedHelper> logger, IDistributedCache cache,
        IOptions<CacheSettings> configuration)
    {
        _cache = cache;
        _logger = logger;
        _cacheSettings = configuration.Value;
    }

    public async Task RemoveCache(string cacheGroupKey, string cacheKey,
        CancellationToken cancellationToken)
    {
        if (cacheGroupKey != null)
        {
            byte[]? cachedGroup = await _cache.GetAsync(cacheGroupKey, cancellationToken);
            if (cachedGroup != null)
            {
                HashSet<string> keysInGroup =
                    JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cachedGroup))!;
                foreach (string key in keysInGroup)
                {
                    await _cache.RemoveAsync(key, cancellationToken);
                }

                await _cache.RemoveAsync(cacheGroupKey, cancellationToken);
                await _cache.RemoveAsync(key: $"{cacheGroupKey}SlidingExpiration", cancellationToken);
            }
        }

        if (cacheKey != null)
            await _cache.RemoveAsync(cacheKey, cancellationToken);
    }

    public async Task<TResponse> GetResponse<TResponse>(string cacheKey,
        TResponse response,
        CancellationToken cancellationToken)
    {
        byte[]? cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse));
            _logger.LogInformation($"Fetched from Cache -> {cacheKey}");
        }

        return response;
    }
    
    public async Task<TResponse> GetResponseAndAddToCache<TResponse>(string cacheGroupKey, string cacheKey,
        TResponse response,
        CancellationToken cancellationToken)
    {
        byte[]? cachedResponse = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedResponse != null)
        {
            response = JsonSerializer.Deserialize<TResponse>(Encoding.Default.GetString(cachedResponse));
            _logger.LogInformation($"Fetched from Cache -> {cacheKey}");
        }
        else
        {
            response = await AddToCache(cacheGroupKey, cacheKey, response, cancellationToken);
        }

        return response;
    }

    public async Task<TResponse> AddToCache<TResponse>(string cacheGroupKey, string cacheKey,
        TResponse response,
        CancellationToken cancellationToken)
    {
        DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();

        if (_cacheSettings.SlidingExpiration != 0)
        {
            cacheOptions.SlidingExpiration = TimeSpan.FromMinutes(_cacheSettings.SlidingExpiration);
        }

        if (_cacheSettings.AbsoluteExpiration != 0)
        {
            cacheOptions.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_cacheSettings.AbsoluteExpiration);
        }
        
        byte[] serializeData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

        await _cache.SetAsync(cacheKey, serializeData, cacheOptions, cancellationToken);

        if (cacheGroupKey != null)
        {
            await _addCacheKeyToGroupAsync(cacheGroupKey, cacheKey, cancellationToken);
        }

        return response;
    }


    private async Task _addCacheKeyToGroupAsync(string cacheGroupKey, string cacheKey,
        CancellationToken cancellationToken = default)
    {
        DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
        int? cacheGroupSlidingExpirationValue = null;
        
        if (_cacheSettings.SlidingExpiration != 0)
        {
            cacheOptions.SlidingExpiration = TimeSpan.FromMinutes(_cacheSettings.SlidingExpiration);
            
            byte[]? cacheGroupSlidingExpirationCache =
                await _cache.GetAsync($"{cacheGroupKey}SlidingExpiration", cancellationToken);
            
            if (cacheGroupSlidingExpirationCache != null)
                cacheGroupSlidingExpirationValue =
                    Convert.ToInt32(Encoding.Default.GetString(cacheGroupSlidingExpirationCache));
            
            if (cacheGroupSlidingExpirationValue == null ||
                ((TimeSpan)cacheOptions.SlidingExpiration).TotalSeconds > cacheGroupSlidingExpirationValue)
                cacheGroupSlidingExpirationValue = Convert.ToInt32(((TimeSpan)cacheOptions.SlidingExpiration).TotalSeconds);
            
        }

        if (_cacheSettings.AbsoluteExpiration != 0)
        {
            cacheOptions.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_cacheSettings.AbsoluteExpiration);
        }
         
        byte[]? cacheGroupCache = await _cache.GetAsync(cacheGroupKey, cancellationToken);
        HashSet<string> cacheKeysInGroup;

        if (cacheGroupCache != null)
        {
            cacheKeysInGroup = JsonSerializer.Deserialize<HashSet<string>>(Encoding.Default.GetString(cacheGroupCache));
            if (!cacheKeysInGroup.Contains(cacheKey))
                cacheKeysInGroup.Add(cacheKey);
        }
        else
        {
            cacheKeysInGroup = new HashSet<string> { cacheKey };
        }

        byte[] newCacheGroupCache = JsonSerializer.SerializeToUtf8Bytes(cacheKeysInGroup);
        
        await _cache.SetAsync(cacheGroupKey, newCacheGroupCache, cacheOptions, cancellationToken);
        if (cacheGroupSlidingExpirationValue != null)
        {
            byte[] serializeCachedGroupSlidingExpirationData =
                JsonSerializer.SerializeToUtf8Bytes(cacheGroupSlidingExpirationValue);
            await _cache.SetAsync($"{cacheGroupKey}SlidingExpiration", serializeCachedGroupSlidingExpirationData,
                cacheOptions, cancellationToken);
        }
        _logger.LogInformation($"Added to Cache -> {cacheGroupKey} and {cacheGroupKey}SlidingExpiration");
    }
}