using Core.Redis.Constants;
using Core.Redis.Helpers;
using IdentityService.Persistance.Abstract.Repositories;
using Nest;

namespace IdentityService.Application.Manager.RefreshTokenManager;

public class RefreshTokenManager : IRefreshTokenManager
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IDistributedHelper _distributedHelper;

    public RefreshTokenManager(IRefreshTokenRepository refreshTokenRepository,IDistributedHelper distributedHelper)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _distributedHelper=distributedHelper;
    }

    #region Implementation of IRefreshTokenManager

    public async Task SetActiveRefreshTokenToRedis()
    {
        var refreshTokens = await _refreshTokenRepository.GetListForRedis();
        await _distributedHelper.RemoveCache(RedisConstants.Jwt, string.Empty, CancellationToken.None);
        foreach (var item in refreshTokens)
        {
            await _distributedHelper.AddToCache(
                RedisConstants.Jwt,
                item.UserId,
                item,
                CancellationToken.None);
        }
    }

    #endregion
}