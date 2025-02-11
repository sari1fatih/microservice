using Core.Hangfire.Settings;
using IdentityService.Application.BackgroundJobs;
using IdentityService.Application.Manager.RefreshTokenManager;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Api.ServiceRegistration.BackgroundJobs;

 
public class ActiveRefreshTokenSetRedisServiceBackgroundJobWorker  : BackgroundJobWorker, IActiveRefreshTokenSetRedisServiceBackgroundJobWorker
{
    private readonly IRefreshTokenManager _refreshTokenManager;
    public ActiveRefreshTokenSetRedisServiceBackgroundJobWorker(IServiceProvider serviceProvider) : base(jobId: "set-active-refresh-token",
        cronExp: null, queueName: "set-active-refresh-token")
    { 
        _refreshTokenManager = serviceProvider.GetRequiredService<IRefreshTokenManager>();
    }

    public override Task Perform(CancellationToken cancellationToken)
    {
         _refreshTokenManager.SetActiveRefreshTokenToRedis();
        
        return Task.CompletedTask;
    }
}