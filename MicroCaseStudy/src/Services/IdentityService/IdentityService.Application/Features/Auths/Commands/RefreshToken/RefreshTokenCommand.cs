using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Pipelines.Transaction;
using Core.Redis.MediaR;
using Core.Security.JWT.Dtos;
using Core.WebAPI.Appsettings.Constants;
using Core.WebAPI.Appsettings.Wrappers;
using IdentityService.Application.Features.Auths.Rules;
using IdentityService.Application.Manager.AuthManager;
using IdentityService.Application.Manager.UserManager;
using IdentityService.Domain.Entities;
using MediatR;

namespace IdentityService.Application.Features.Auths.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Response<RefreshedTokensDto>>, ITransactionalRequest,
    IJwtRemoveRedisCachableRequest, IJwtAddRedisCachableRequest
{
    public string Jti { get; set; }
    public string IpAddress { get; set; }
    [JsonIgnore]
    public string Jwt { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    [JsonIgnore]
    public bool IsDeletedUserAll { get; set; }
    [JsonIgnore]
    public DateTime ExpiresDate { get; set; }

    public RefreshTokenCommand()
    {
        Jti = string.Empty;
        IpAddress = string.Empty;
    }

    public RefreshTokenCommand(string refreshToken, string ipAddress)
    {
        Jti = refreshToken;
        IpAddress = ipAddress;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<RefreshedTokensDto>>
    {
        private readonly IAuthManager _authManager;
        private readonly IUserManager _userManager;
        private readonly AuthBusinessRules _authBusinessRules;
        private readonly IBaseService _baseService;
        private readonly IMapper _mapper;
        private readonly IJwtAddRedisCachableRequest _jwtAddRedisCachableRequest;
        private readonly IJwtRemoveRedisCachableRequest _jwtRemoveRedisCachableRequest;

        public RefreshTokenCommandHandler(IAuthManager authManager, IUserManager userManager,
            AuthBusinessRules authBusinessRules, IBaseService baseService, IMapper mapper,
            IJwtRemoveRedisCachableRequest jwtRemoveRedisCachableRequest,
            IJwtAddRedisCachableRequest jwtAddRedisCachableRequest)
        {
            _authManager = authManager;
            _userManager = userManager;
            _authBusinessRules = authBusinessRules;
            _baseService = baseService;
            _mapper = mapper;
            _jwtAddRedisCachableRequest = jwtAddRedisCachableRequest;
            _jwtRemoveRedisCachableRequest = jwtRemoveRedisCachableRequest;
        }

        public async Task<Response<RefreshedTokensDto>> Handle(RefreshTokenCommand request, CancellationToken
            cancellationToken)
        {
            Domain.Entities.RefreshToken?
                refreshToken = await _authManager.GetRefreshTokenByJti(request.Jti);
            await _authBusinessRules.RefreshTokenShouldBeExists(refreshToken);

            if (refreshToken!.RevokedDate != null)
                await _authManager.RevokeDescendantRefreshTokens(
                    refreshToken,
                    request.IpAddress,
                    reason: $"Attempted reuse of revoked ancestor token: {refreshToken.Jti}"
                );
            await _authBusinessRules.RefreshTokenShouldBeActive(refreshToken);

            User? user = await _userManager.GetAsync(
                predicate: u => u.Id == refreshToken.UserId,
                cancellationToken: cancellationToken
            );

            await _authBusinessRules.UserShouldBeExistsWhenSelected(user);

            AccessToken createdAccessToken = await _authManager.CreateAccessToken(user!);
            refreshToken.UpdatedBy = user?.Id;
            var newRefresh = await _authManager.RotateRefreshToken(
                user: user!,
                refreshToken,
                request.IpAddress,
                createdAccessToken.Jti
            );

            Domain.Entities.RefreshToken newRefreshToken = _mapper.Map<Domain.Entities.RefreshToken>(newRefresh);

            newRefreshToken.CreatedBy = user.Id;
            newRefreshToken.IsActive = true;
            Domain.Entities.RefreshToken addedRefreshToken = await _authManager.AddRefreshToken(newRefreshToken);

            RefreshedTokensDto refreshedTokensDto =
                new() { AccessToken = createdAccessToken, RefreshToken = newRefresh.Token };
            _jwtAddRedisCachableRequest.UserId = refreshToken.UserId.ToString();
            _jwtAddRedisCachableRequest.ExpiresDate = newRefreshToken.ExpiresDate;
            _jwtAddRedisCachableRequest.Jwt = newRefreshToken.Jti;
            
            _jwtRemoveRedisCachableRequest.Jwt = request.Jti;
            _jwtRemoveRedisCachableRequest.UserId =refreshToken.UserId.ToString();
            _jwtRemoveRedisCachableRequest.IsDeletedUserAll = false;
            return _baseService.CreateSuccessResult<RefreshedTokensDto>(refreshedTokensDto,
                InternalsConstants.Success);
        }
    }
}