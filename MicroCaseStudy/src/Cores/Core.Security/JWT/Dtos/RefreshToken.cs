using System.Text.Json.Serialization;

namespace Core.Security.JWT.Dtos;

public class RefreshToken<TRefreshTokenId, TUserId>
{
    public TRefreshTokenId Id { get; set; }
    public TUserId UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresDate { get; set; }
    public string CreatedByIp { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsActive { get; set; } = true;
    [JsonIgnore]
    public string Jti { get; set; }
    public RefreshToken()
    {
        UserId = default!;
        Token = string.Empty;
        CreatedByIp = string.Empty;
    }

    public RefreshToken(TRefreshTokenId id, TUserId userId, string token, DateTime expiresDate, string createdByIp)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpiresDate = expiresDate;
        CreatedByIp = createdByIp;
    }
}