using Core.Redis.Constants;
using Core.Redis.Dtos;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.Redis.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.Caching;

public class JwtRemovingCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IJwtRemoveRedisCachableRequest
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedHelper _cache;
    private readonly IJwtRemoveRedisCachableRequest _jwtRemoveRedisCachableRequest;
    private readonly ILogger<JwtRemovingCachingBehavior<TRequest, TResponse>> _logger;

    public JwtRemovingCachingBehavior(ILogger<JwtRemovingCachingBehavior<TRequest, TResponse>> logger,
        IDistributedHelper cache,
        IJwtRemoveRedisCachableRequest jwtRemoveRedisCachableRequest,
        IConfiguration configuration)
    {
        _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ??
                         throw new InvalidOperationException();
        _cache = cache;
        _jwtRemoveRedisCachableRequest = jwtRemoveRedisCachableRequest;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = await next();

        JwtRedisDto jwtRedisDto = new JwtRedisDto();

        jwtRedisDto = await _cache.GetResponse(_jwtRemoveRedisCachableRequest.UserId.ToString(),
            jwtRedisDto, cancellationToken);

        if (_jwtRemoveRedisCachableRequest.IsDeletedUserAll)
        {
            await _cache.RemoveCache(RedisConstants.Jwt,
                _jwtRemoveRedisCachableRequest.UserId.ToString(), cancellationToken);
            return response;
        }
        
        jwtRedisDto.JwtExpireDateDtos.RemoveAll(x =>
            x.Jwt == _jwtRemoveRedisCachableRequest.Jwt.ToString() || 
            x.ExpiresDate < DateTime.Now
            
            );
        if (jwtRedisDto.JwtExpireDateDtos.Count != 0)
        {
            await _cache.AddToCache(
                RedisConstants.Jwt,
                _jwtRemoveRedisCachableRequest.UserId.ToString(),
                jwtRedisDto,
                cancellationToken);
        }
        else
            await _cache.RemoveCache(RedisConstants.Jwt,
                _jwtRemoveRedisCachableRequest.UserId.ToString(), cancellationToken);

        return response;
    }
}