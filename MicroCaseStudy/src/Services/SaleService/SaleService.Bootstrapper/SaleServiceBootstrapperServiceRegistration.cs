using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaleService.Api.ServiceRegistration;
using SaleService.Application;
using SaleService.Persistance;

namespace SaleService.Bootstrapper;

public static class SaleServiceBootstrapperServiceRegistration
{
    public static void AddSaleServiceBootstrapperServiceRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSaleServicePersistanceServiceRegistration(configuration);
        services.AddSaleServiceApplicationServiceRegistration(configuration); 
        services.AddSaleServiceApiServiceRegistration(configuration); 
    }
}