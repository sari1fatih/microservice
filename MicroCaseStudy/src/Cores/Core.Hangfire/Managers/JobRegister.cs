using Core.Hangfire.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Hangfire.Managers;

public static class JobRegister
{
    public static void AddBackgroundJob<TService, TImplement>(this IServiceCollection services) 
        where TService : class, IBackGroundJobWorker
        where TImplement : class, TService
    { 
        var serviceProvider = services.BuildServiceProvider();
        var registerer = serviceProvider.GetRequiredService<IHangfireBackgroundJobManager>();
        var service = serviceProvider.GetRequiredService<TService>();
        
        registerer.RegisterJob(service);
    }
}