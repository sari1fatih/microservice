using Core.Security.EmailAuthenticator;
using Core.Security.JWT;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Security;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityServices<TRefreshTokenId, TUserId>(
        this IServiceCollection services,
        TokenOptions tokenOptions
    )
    {
        services.AddScoped<
            ITokenHelper<TRefreshTokenId, TUserId>,
            TokenHelper<TRefreshTokenId, TUserId>
        >(_ => new TokenHelper<TRefreshTokenId, TUserId>(tokenOptions));
        
        
        services.AddScoped<IEmailAuthenticatorHelper, EmailAuthenticatorHelper>();

        return services;
    }
}