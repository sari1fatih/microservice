using Core.Security.JWT.Dtos;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Manager.AuthManager;

public interface IAuthManager
{
    public Task<AccessToken> CreateAccessToken(User user);
    public Task<RefreshToken<int, int>> CreateRefreshToken(User user, string ipAddress, string jti);
    public Task<RefreshToken?> GetRefreshTokenByJti(string jti);
    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken); 
    public Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);
    public Task RevokeRefreshToken(RefreshToken refreshToken, string ipAddress, string? reason = null, string? replacedByToken = null);
    public Task<RefreshToken<int, int>> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress,string newJti);
}