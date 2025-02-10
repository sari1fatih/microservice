using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Persistance.Abstract.Repositories;
using IdentityService.Persistance.Context;
using IdentityService.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Persistance;

public static class IdentityServicePersistanceServiceRegistration
{
     public static IServiceCollection AddIdentityServicePersistanceServiceRegistration(this IServiceCollection service,
        IConfiguration configuration)
     {
        service.AddDbContext<IdentityServiceDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("IdentityService")));

        service.AddCoreWebAPIAppsettingServiceRegistration();
        
        service.AddScoped<IRoleRepository, RoleRepository>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<IUserRoleRepository, UserRoleRepository>();
        service.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        return service;
    }
}