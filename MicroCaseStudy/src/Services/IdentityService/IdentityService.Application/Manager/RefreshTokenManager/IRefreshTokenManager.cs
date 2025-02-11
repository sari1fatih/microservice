namespace IdentityService.Application.Manager.RefreshTokenManager;

public interface IRefreshTokenManager
{ 
    Task SetActiveRefreshTokenToRedis();
}