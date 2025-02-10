using CustomerService.Api.ServiceRegistration;
using CustomerService.Application;
using CustomerService.Persistance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Bootstrapper;

public static class CustomerServiceBootstrapperServiceRegistration
{
    public static void AddCustomerServiceBootstrapperServiceRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
           
        services.AddCustomerServicePersistanceServiceRegistration(configuration);
        services.AddCustomerServiceApplicationServiceRegistration(configuration); 
        services.AddCustomerServiceApiServiceRegistration(configuration); 
    }
} 