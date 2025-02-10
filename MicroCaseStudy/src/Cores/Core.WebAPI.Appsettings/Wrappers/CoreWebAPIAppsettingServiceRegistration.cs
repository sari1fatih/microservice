using Microsoft.Extensions.DependencyInjection;

namespace Core.WebAPI.Appsettings.Wrappers;

public static class CoreWebAPIAppsettingServiceRegistration
{
    public static IServiceCollection AddCoreWebAPIAppsettingServiceRegistration(this IServiceCollection services)
    {
        services.AddSingleton<IBaseService, BaseService>();
        return services;
    }
}