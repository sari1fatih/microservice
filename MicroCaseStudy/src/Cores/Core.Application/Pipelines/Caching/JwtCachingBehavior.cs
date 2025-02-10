using System.Text;
using System.Text.Json;
using Core.Redis.Constants;
using Core.Redis.Dtos;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.Redis.Settings;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.Caching;

public class JwtCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IJwtAddRedisCachableRequest
{
    private readonly CacheSettings _cacheSettings;
    private readonly IDistributedHelper _cache;
    private readonly IJwtAddRedisCachableRequest _jwtAddRedisCachableRequest;
    private readonly ILogger<JwtCachingBehavior<TRequest, TResponse>> _logger;

    public JwtCachingBehavior(ILogger<JwtCachingBehavior<TRequest, TResponse>> logger, 
        IDistributedHelper cache,
        IJwtAddRedisCachableRequest jwtAddRedisCachableRequest,
        IConfiguration configuration)
    {
        _cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>() ??
                         throw new InvalidOperationException();
        _cache = cache;
        _jwtAddRedisCachableRequest= jwtAddRedisCachableRequest;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        TResponse response = await next();
        
        JwtRedisDto jwtRedisDto = new JwtRedisDto();

        jwtRedisDto = await _cache.GetResponse(_jwtAddRedisCachableRequest.UserId,
            jwtRedisDto, cancellationToken);
        jwtRedisDto.UserId = _jwtAddRedisCachableRequest.UserId.ToString();
        
        JwtExpireDateDto jwtExpireDateDto = new JwtExpireDateDto();

        jwtExpireDateDto.Jwt = _jwtAddRedisCachableRequest.Jwt;
        jwtExpireDateDto.ExpiresDate = _jwtAddRedisCachableRequest.ExpiresDate;

        jwtRedisDto.JwtExpireDateDtos.RemoveAll(x => x.ExpiresDate < DateTime.Now);
        jwtRedisDto.JwtExpireDateDtos.Add(jwtExpireDateDto);
        
        await _cache.AddToCache(
            RedisConstants.Jwt,
            jwtRedisDto.UserId,
            jwtRedisDto,
            cancellationToken);
        
        return response;
    }
}