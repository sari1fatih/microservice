using System.Security.Claims;
using AutoMapper;
using Core.Application.Enums;
using Core.Security.JWT;
using Core.Security.JWT.Dtos;
using IdentityService.Domain.Entities;
using IdentityService.Persistance.Abstract.Repositories;
using Microsoft.Extensions.Options;

namespace IdentityService.Application.Manager.AuthManager;

public class AuthManager : IAuthManager
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper<int, int> _tokenHelper;
    private readonly TokenOptions _tokenOptions;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;

    public AuthManager(IRefreshTokenRepository refreshTokenRepository, ITokenHelper<int, int> tokenHelper,
        IOptions<TokenOptions> _optionsTokens, IUserRoleRepository userRoleRepository, IMapper
            mapper)
    {
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        _tokenOptions = _optionsTokens.Value;
    }

    #region Implementation of IAuthService

    public async Task<AccessToken> CreateAccessToken(User user)
    {
        IList<Role> roles =
            await _userRoleRepository.GetRoleClaimsByUserIdAsync(user.Id);
        
        List<Claim> listClaims = new List<Claim>();
        listClaims.Add(new Claim(CustomClaimKeys.Id, user.Id.ToString()));
        listClaims.Add(new Claim(CustomClaimKeys.Mail, user.Email));
        listClaims.Add(new Claim(CustomClaimKeys.Username, user.Username));
        listClaims.Add(new Claim(CustomClaimKeys.Name, user.Name));
        if (user.Surname != null) listClaims.Add(new Claim(CustomClaimKeys.Surname, user.Surname)); 
        
       var accessToken = await _tokenHelper.CreateToken(
            listClaims,
            roles.Select(op => (op.RoleValue)).ToList()
        );

        return accessToken;
    }

    public Task<RefreshToken<int, int>> CreateRefreshToken(User user, string ipAddress, string jti)
    {
        RefreshToken<int, int> coreRefreshToken = _tokenHelper.CreateRefreshToken(
            user.Id,
            ipAddress,
            jti
        );
        return Task.FromResult(coreRefreshToken);
    }
 
    public async Task<RefreshToken?> GetRefreshTokenByJti(string jti)
    {
        RefreshToken? refreshToken =
            await _refreshTokenRepository.GetAsync(predicate: r =>
                r.Jti == jti && 
                r.ExpiresDate >= DateTime.UtcNow &&
                r.IsActive == true
            );
        return refreshToken;
    }

    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken, TableCreatedParameters
            .CreatedAtPropertyName, TableCreatedParameters.CreatedByPropertyName);
        return addedRefreshToken;
    }

  

    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        RefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Jti == refreshToken.ReplacedByJti
        );

        if (childToken?.RevokedDate != null && childToken.ExpiresDate <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }

    public async Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string?
        replacedByJti = null)
    {
        refreshToken.IsActive = false;
        refreshToken.RevokedDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = reason;
        refreshToken.ReplacedByJti = replacedByJti;
        await _refreshTokenRepository.UpdateAsync(refreshToken, TableUpdatedParameters.UpdatedAtPropertyName,
            TableUpdatedParameters.UpdatedByPropertyName);
    }

    public async Task<RefreshToken<int, int>> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress,string newJti)
    {
        RefreshToken<int, int> newCoreRefreshToken = _tokenHelper.CreateRefreshToken(
            user.Id,
            ipAddress, 
            newJti
        );
        //RefreshToken newRefreshToken = _mapper.Map<RefreshToken>(newCoreRefreshToken);
        await RevokeRefreshToken(refreshToken, ipAddress, reason: "Replaced by new token", newJti);
        return newCoreRefreshToken;
    }

    #endregion
}