using System.IdentityModel.Tokens.Jwt;
using Core.Security.JWT;
using IdentityService.Application.Manager.AuthManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Api.ServiceRegistration.Handlers;

public class TokenAuthorizationRequirement: IAuthorizationRequirement
{
    
}

public class TokenAuthorizationHandler : AuthorizationHandler<TokenAuthorizationRequirement>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor; 
    private readonly ITokenHelper<int, int> _tokenHelper;

    public TokenAuthorizationHandler(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor,
        ITokenHelper<int, int> tokenHelper)
    {
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
        _tokenHelper = tokenHelper;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        TokenAuthorizationRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!_tokenHelper.ValidateToken(token))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.CompleteAsync();
            context.Fail();
            return;
        }

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var refreshJwtToken = handler.ReadJwtToken(token);

            var jti = refreshJwtToken.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Jti)?.Value;
            IAuthManager authManager = _serviceProvider.GetRequiredService<IAuthManager>();

            var isToken = await authManager.GetRefreshTokenByJti(jti);

            if (isToken == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.CompleteAsync();
                context.Fail();
                return;
            }
        }
        
        context.Succeed(requirement);
    }
}