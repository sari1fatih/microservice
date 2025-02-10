using Core.WebAPI.Appsettings.Wrappers;
using CustomerService.Persistance.Abstract.Repositories;
using CustomerService.Persistance.Context;
using CustomerService.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Persistance;

public static class CustomerServicePersistanceServiceRegistration
{
    public static IServiceCollection AddCustomerServicePersistanceServiceRegistration(this IServiceCollection service,
        IConfiguration configuration)
    { 
        service.AddDbContext<CustomerServiceDbContext>(options => options.UseNpgsql(configuration.GetConnectionString
            ("CustomerService")));
   
        service.AddCoreWebAPIAppsettingServiceRegistration();
           
        service.AddScoped<ICustomerNoteRepository, CustomerNoteRepository>();
        service.AddScoped<ICustomerRepository, CustomerRepository>();
           
        return service;
    }
}