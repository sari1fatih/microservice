using Core.WebAPI.Appsettings.Wrappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaleService.Persistance.Abstract.Repositories;
using SaleService.Persistance.Context;
using SaleService.Persistance.Repositories;

namespace SaleService.Persistance;

public static class SaleServicePersistanceServiceRegistration
{
    public static IServiceCollection AddSaleServicePersistanceServiceRegistration(this IServiceCollection service,
        IConfiguration configuration)
    {
        service.AddDbContext<SaleServiceDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("SaleService")));
   
        service.AddCoreWebAPIAppsettingServiceRegistration();
           
        service.AddScoped<IParameterGroupRepository, ParameterGroupRepository>();
        service.AddScoped<IParameterRepository, ParameterRepository>();
        service.AddScoped<ISaleDetailRepository, SaleDetailRepository>();
        service.AddScoped<ISaleRepository, SaleRepository>();
           
        return service;
    }
}
 