using System.IdentityModel.Tokens.Jwt;
using Core.Redis.Constants;
using Core.Redis.Dtos;
using Core.Redis.Helpers;
using Core.Security.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SaleService.Api.ServiceRegistration.Handlers;

public class TokenAuthorizationRequirement : IAuthorizationRequirement
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

            var userId = refreshJwtToken.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Id)?.Value;

            IDistributedHelper distributedHelper = _serviceProvider.GetRequiredService<IDistributedHelper>();

            JwtRedisDto jwtRedisDto = new JwtRedisDto();

            jwtRedisDto = await distributedHelper.GetResponse(userId,
                jwtRedisDto, CancellationToken.None);

            var isOld = jwtRedisDto.JwtExpireDateDtos.Any(x => x.ExpiresDate < DateTime.Now);

            if (isOld)
            {
                jwtRedisDto.JwtExpireDateDtos.RemoveAll(x => x.ExpiresDate < DateTime.Now);

                await distributedHelper.AddToCache(
                    RedisConstants.Jwt,
                    jwtRedisDto.UserId,
                    jwtRedisDto,
                    CancellationToken.None);
            }

            var isJwtActive = jwtRedisDto.JwtExpireDateDtos.FirstOrDefault(x => x.Jwt == jti);
            if (isJwtActive == null)
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