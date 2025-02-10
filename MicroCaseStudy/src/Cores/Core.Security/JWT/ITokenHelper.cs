using System.Security.Claims;
using Core.Security.JWT.Dtos;

namespace Core.Security.JWT;

public interface ITokenHelper<TRefreshTokenId,TUserId>
{
    public Task<AccessToken> CreateToken(List<Claim> claims,IEnumerable<string> roles);

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(TUserId user, string ipAddress,
        string jti);

    public bool ValidateToken(string token);
}