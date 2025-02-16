using IdentityService.Api.ServiceRegistration;
using IdentityService.Application;
using IdentityService.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Bootstrapper;

public static class IdentityServiceBootstrapperServiceRegistration
{
    public static void AddIdentityServiceBootstrapperServiceRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
        //services.AddHangfireServiceRegistration();
        services.AddIdentityServicePersistanceServiceRegistration(configuration);
        services.AddIdentityServiceApplicationServiceRegistration(configuration); 
        services.AddIdentityServiceApiServiceRegistration(configuration); 
    }
}