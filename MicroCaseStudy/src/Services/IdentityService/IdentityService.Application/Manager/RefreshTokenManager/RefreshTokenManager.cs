using Core.Redis.Constants;
using Core.Redis.Helpers;
using IdentityService.Persistance.Abstract.Repositories;

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
        var data = await _refreshTokenRepository.GetListForRedis();
        
        Parallel.ForEach(data, item =>
        {
            _distributedHelper.AddToCache(
                RedisConstants.Jwt,
                item.UserId,
                item,
                CancellationToken.None);
        }); 
    }

    #endregion
}