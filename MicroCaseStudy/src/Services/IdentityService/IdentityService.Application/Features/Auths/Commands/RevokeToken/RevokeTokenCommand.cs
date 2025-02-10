using System.Text.Json.Serialization;
using Core.Application.Pipelines.Transaction;
using Core.Redis.Helpers;
using Core.Redis.MediaR;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.AuthManager;
using MediatR;

namespace IdentityService.Application.Features.Auths.Commands.RevokeToken;

public class RevokeTokenCommand : IRequest<Response<RevokedTokenDto>>, ITransactionalRequest, IJwtRemoveRedisCachableRequest
{
    public string Jti { get; set; }
    public string IpAddress { get; set; } 
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public bool IsDeletedUserAll { get; set; }

    public RevokeTokenCommand()
    {
        Jti = string.Empty;
        IpAddress = string.Empty;
    }

    public RevokeTokenCommand(string token, string ipAddress)
    {
        Jti = token;
        IpAddress = ipAddress;
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Response<RevokedTokenDto>>
    {
        private readonly IAuthManager _authManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IJwtRemoveRedisCachableRequest _jwtRemoveRedisCachableRequest;
        private readonly IBaseService _baseService;
        private readonly IDistributedHelper _distributedHelper; 
        private readonly IUserSession<int> _userSession;

        public RevokeTokenCommandHandler(IAuthManager authManager, AuthBusinessRules authBusinessRules,
            IDistributedHelper distributedHelper,
            IJwtRemoveRedisCachableRequest jwtRemoveRedisCachableRequest,
            IUserSession<int> userSession,
            IBaseService baseService)
        {
            _jwtRemoveRedisCachableRequest = jwtRemoveRedisCachableRequest;
            _authManager = authManager;
            _authBusinessRules = authBusinessRules;
            _distributedHelper = distributedHelper;
            _baseService = baseService;
            _userSession = userSession;
        }

        public async Task<Response<RevokedTokenDto>> Handle(RevokeTokenCommand request, CancellationToken
            cancellationToken)
        {
            Domain.Entities.RefreshToken? refreshToken = await _authManager.GetRefreshTokenByJti(request.Jti);
            await _authBusinessRules.RefreshTokenShouldBeExists(refreshToken);
            await _authBusinessRules.RefreshTokenShouldBeActive(refreshToken!);
            
            await _authManager.RevokeRefreshToken(refreshToken: refreshToken!, request.IpAddress,
                reason: "Revoked without replacement");
 
            _jwtRemoveRedisCachableRequest.Jwt = _userSession.Jti;
            _jwtRemoveRedisCachableRequest.UserId = _userSession.UserId.ToString(); 
            _jwtRemoveRedisCachableRequest.IsDeletedUserAll = false; 
            
            return _baseService.CreateSuccessResult<RevokedTokenDto>(null,
                InternalsConstants.Success);
        }
    } 
}