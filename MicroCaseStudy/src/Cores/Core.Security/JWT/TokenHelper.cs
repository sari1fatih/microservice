using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Core.Security.Extensions;
using Core.Security.JWT.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace Core.Security.JWT;

public class TokenHelper<TRefreshTokenId, TUserId> : ITokenHelper<TRefreshTokenId, TUserId>
{
    private readonly TokenOptions _tokenOptions;
    private readonly RSA _privateKey;
    private readonly RSA _publicKey;

    public TokenHelper(TokenOptions tokenOptions)
    {
        _tokenOptions = tokenOptions;
        _privateKey = RSA.Create();
        _privateKey.ImportFromPem(_tokenOptions.PrivateKey);

        _publicKey = RSA.Create();
        _publicKey.ImportFromPem(_tokenOptions.PublicKey);
    }

     public async Task<AccessToken> CreateToken(List<Claim> listClaims, IEnumerable<string> roles)
    {
        var jti = await Task.FromResult(Guid.NewGuid().ToString());
        listClaims.Add(new Claim(CustomClaimKeys.Typ,CustomClaimValues.TypBearer));
        listClaims.Add(new Claim(CustomClaimKeys.Jti,jti));
       
        DateTime accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);
        var key = new RsaSecurityKey(_privateKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha512);
        var jwt = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: SetClaims(listClaims, roles), 
            expires: accessTokenExpiration,
            signingCredentials: creds);
        
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);

        return new AccessToken() { Token = token, ExpiresDate = accessTokenExpiration, Jti = jti};
    }

    public RefreshToken<TRefreshTokenId, TUserId> CreateRefreshToken(TUserId userId, string ipAddress,
        string jti)
    { 
        List<Claim> listClaims = new List<Claim>();
        listClaims.Add(new Claim(CustomClaimKeys.Id, userId.ToString()));
        
        listClaims.Add(new Claim(CustomClaimKeys.Typ,CustomClaimValues.TypRefresh));
        listClaims.Add(new Claim(CustomClaimKeys.Jti,jti));
   
        DateTime accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.RefreshTokenTTL);
        var key = new RsaSecurityKey(_privateKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha512);
        var jwt = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience, 
            claims: listClaims.Select(x => new Claim(x.Type, x.Value)),
            expires: accessTokenExpiration,
            signingCredentials: creds);
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        string? token = jwtSecurityTokenHandler.WriteToken(jwt);
        
        return new RefreshToken<TRefreshTokenId, TUserId>()
        {
            Jti = jti,
            UserId = userId,
            Token = token,
            ExpiresDate = accessTokenExpiration,
            CreatedByIp = ipAddress
        };
    }

    public bool ValidateToken(string token)
    {
        var rsaSecurityKey = new RsaSecurityKey(_publicKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = rsaSecurityKey, // Public key ile doğrulama yapacağız
            ValidateIssuer = true,
            ValidIssuer = _tokenOptions.Issuer, // Token oluşturulurken belirtilen Issuer
            ValidateAudience = true,
            ValidAudience = _tokenOptions.Audience, // Token oluşturulurken belirtilen Audience
            ValidateLifetime = true, // Token'ın süresi dolmuş mu kontrol edilecek
            ClockSkew = TimeSpan.Zero // Geçerlilik süresinde tolerans tanımama
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
            return true;
        }
        catch (Exception e)
        {
            return false;
        } 
    }

    protected virtual IEnumerable<Claim> SetClaims(IEnumerable<Claim> listClaims, IEnumerable<string> roles)
    {
        List<Claim> claims = [];
        claims.AddIEnumerableClaims(listClaims);
        claims.AddRoles(roles.Select(c => c).ToArray());
        return claims.ToImmutableList();
    }
}