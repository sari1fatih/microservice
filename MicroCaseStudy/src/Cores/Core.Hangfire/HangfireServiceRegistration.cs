using Core.Hangfire.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Hangfire;

public static class HangfireServiceRegistration
{
    public static IServiceCollection AddHangfireServiceRegistration(
        this IServiceCollection services
    )
    {
        services.AddScoped<IHangfireBackgroundJobManager, HangfireBackgroundJobManager>();
        return services;
    }
}