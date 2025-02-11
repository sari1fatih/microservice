using System.Reflection;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Application.Pipelines.Validation;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Serilog;
using Core.CrossCuttingConcerns.Serilog.Loggers;
using Core.ElasticSearch;
using Core.Mailing;
using Core.Mailing.MailKit;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.Redis.Settings;
using Core.Security;
using Core.Security.JWT;
using Core.WebAPI.Appsettings;
using FluentValidation;
using IdentityService.Application.Manager.AuthManager;
using IdentityService.Application.Manager.RefreshTokenManager;
using IdentityService.Application.Manager.RoleManager;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Application.Manager.UserRoleManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application;

public static class IdentityServiceApplicationServiceRegistration
{
    public static IServiceCollection AddIdentityServiceApplicationServiceRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDistributedHelper, DistributedHelper>();
        services.AddScoped<IJwtAddRedisCachableRequest, JwtAddRedisCachableRequest>();
        services.AddScoped<IJwtRemoveRedisCachableRequest , JwtRemoveRedisCachableRequest>();
        services.AddCoreElasticSearchRegistration(configuration);

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        var mailSettings = configuration.GetSection(nameof(MailSettings)).Get<MailSettings>();
        
        services.Configure<GeneralSettings>(configuration.GetSection(nameof(GeneralSettings)));
        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));
         
        services.Configure<TokenOptions>(configuration.GetSection(nameof(TokenOptions)));

        var tokenOptions = configuration.GetSection(nameof(TokenOptions)).Get<TokenOptions>();
        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(JwtCachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(JwtRemovingCachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
        });;
        services.AddSingleton<IMailService, MailKitMailService>(_ => new MailKitMailService(mailSettings));
        
        services.AddSecurityServices<int, int>(tokenOptions);
         
        services.AddSingleton<LoggerServiceBase, PostgreSqlLogger>();
        
        services.AddScoped<IAuthManager, AuthManager>(); 
        services.AddScoped<IRoleManager, RoleManager>();
        services.AddScoped<IUserRoleManager, UserRoleManager>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IRefreshTokenManager, RefreshTokenManager>();
        
        services.AddScoped<IUserSession<int>, UserSession<int>>();
        
        return services;
    }
    
    public static IServiceCollection AddSubClassesOfType(
        this IServiceCollection services,
        Assembly assembly,
        Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null
    )
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (Type? item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }
}