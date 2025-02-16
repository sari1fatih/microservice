using System.IdentityModel.Tokens.Jwt;
using Core.Api.Controllers;
using Core.Security.JWT;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Api.Attributes;
using IdentityService.Application.Features.Auths.Commands.Login;
using IdentityService.Application.Features.Auths.Commands.RefreshToken;
using IdentityService.Application.Features.Auths.Commands.Register;
using IdentityService.Application.Features.Auths.Commands.RevokeToken;
using IdentityService.Application.Features.Auths.Commands.VerifyRegister;
using IdentityService.Application.Features.Auths.ResetPassword;
using IdentityService.Application.Features.Auths.VerifyResetPassword;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace IdentityService.Api.Controllers
{
    [ElasticsearchRequestResponse]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : BaseController
    {
        private readonly WebApiConfiguration _configuration;
        private readonly TokenOptions _tokenOptions;

        public AuthsController(IOptions<WebApiConfiguration> webApiConfiguration,
            IOptions<TokenOptions> tokenOptionsConfiguration)
        {
            _configuration = webApiConfiguration.Value;
            _tokenOptions = tokenOptionsConfiguration.Value;
        }


        #region AllowAnonymous

        [EnableRateLimiting("RateLimitIp")] 
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand userForRegisterDto)
        {
            userForRegisterDto.IpAddress = getIpAddress();

            var result = await Mediator.Send(userForRegisterDto);

            return Ok(result);
        }
        [EnableRateLimiting("RateLimitIp")] 
        [HttpPost("VerifyRegister")]
        public async Task<IActionResult> VerifyRegister([FromBody] VerifyForRegisterDto verifyForRegisterDto)
        {
            VerifyRegisterCommand registerCommand = new()
                { VerifyForRegisterDto = verifyForRegisterDto, IpAddress = getIpAddress() };
            var result = await Mediator.Send(registerCommand);

            if (result.ApiResultType == ApiResultType.Success && result.Data?.RefreshToken is not null)
                setRefreshTokenToCookie(result.Data.RefreshToken);
            return Created(uri: "", result);
        }
        
        
        [EnableRateLimiting("RateLimitIp")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForLoginDto)
        {
            LoginCommand loginCommand = new() { UserForLoginDto = userForLoginDto, IpAddress = getIpAddress() };
            var result = await Mediator.Send(loginCommand);
            if (result.ApiResultType == ApiResultType.Success && result.Data?.RefreshToken is not null)
                setRefreshTokenToCookie(result.Data.RefreshToken);

            return Ok(result);
        }

        [Authorize(Policy = "TokenAuthorizationHandler")]
        [EnableRateLimiting("RateLimitIp")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            RefreshTokenCommand refreshTokenCommand =
                new() { Jti = getJtiFromRefreshTokenCookies(string.Empty), IpAddress = getIpAddress() };
            var result = await Mediator.Send(refreshTokenCommand);

            if (result.ApiResultType == ApiResultType.Success && result.Data?.RefreshToken is not null)
                setRefreshTokenToCookie(result.Data.RefreshToken);

            return Created(uri: "", result);
        }

        #endregion AllowAnonymous

        [EnableRateLimiting("RateLimitUserId")]
        [Authorize(Policy = "TokenAuthorizationHandler")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("RevokeToken")]
        public async Task<IActionResult> RevokeToken(
            [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
            string? refreshToken)
        {
            RevokeTokenCommand revokeTokenCommand =
                new() { Jti = getJtiFromRefreshTokenCookies(refreshToken), IpAddress = getIpAddress() };
            return Ok(await Mediator.Send(revokeTokenCommand));
        }

        [EnableRateLimiting("RateLimitUserId")]
        [Authorize(Policy = "TokenAuthorizationHandler")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordCommand()
        {
            ResetPasswordCommand resetPasswordCommand = new ResetPasswordCommand();
            var result = await Mediator.Send(resetPasswordCommand);

            return Ok(result);
        }

        [EnableRateLimiting("RateLimitUserId")]
        [Authorize(Policy = "TokenAuthorizationHandler")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("VerifyResetPassword")]
        public async Task<IActionResult> VerifyResetPasswordCommand(
            [FromBody] VerifyResetPasswordCommand verifyResetPasswordCommand)
        {
            var result = await Mediator.Send(verifyResetPasswordCommand);

            return Ok(result);
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("SetActiveRefreshTokenToRedis")]
        public async Task<IActionResult> SetActiveRefreshTokenToRedis()
        {
            SetActiveRefreshTokenToRedisCommand setActiveRefreshTokenToRedisCommand=new SetActiveRefreshTokenToRedisCommand();
            
            var result = await Mediator.Send(setActiveRefreshTokenToRedisCommand);

            return Ok(result);
        }

        private string getJtiFromRefreshTokenCookies(string? reqRefreshToken)
        {
            reqRefreshToken = string.IsNullOrEmpty(reqRefreshToken)
                ? Request.Cookies["refreshToken"]
                : reqRefreshToken;
            var refreshToken = reqRefreshToken ??
                               throw new ArgumentException("Refresh token is not found in request cookies.");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(refreshToken);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == CustomClaimKeys.Jti)?.Value;
            return jti;
        }

        private void setRefreshTokenToCookie(string refreshToken)
        {
            CookieOptions cookieOptions = new()
                { HttpOnly = true, Expires = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTTL) };
            Response.Cookies.Append(key: "refreshToken", refreshToken, cookieOptions);
        }
    }
}