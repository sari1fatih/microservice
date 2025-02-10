using System.Text.Json.Serialization;

namespace Core.Security.JWT.Dtos;

public class AccessToken
{
    public string Token { get; set; }
    [JsonIgnore]
    public string Jti { get; set; }
    public DateTime ExpiresDate { get; set; } 

    public AccessToken()
    {
        Token = string.Empty;
    }

    public AccessToken(string token, string jti, DateTime expiresDate)
    {
        Token = token;
        Jti = jti;
        ExpiresDate = expiresDate;
    }
}