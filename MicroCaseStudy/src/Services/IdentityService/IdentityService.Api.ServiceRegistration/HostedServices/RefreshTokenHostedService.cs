using IdentityService.Application.Manager.RefreshTokenManager;
using Microsoft.Extensions.Hosting;

namespace IdentityService.Api.ServiceRegistration.HostedServices;

public class RefreshTokenHostedService: IHostedService
{
    private readonly IRefreshTokenManager _refreshTokenManager;

    public RefreshTokenHostedService(IRefreshTokenManager refreshTokenManager)
    {
        _refreshTokenManager = refreshTokenManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _refreshTokenManager.SetActiveRefreshTokenToRedis();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}