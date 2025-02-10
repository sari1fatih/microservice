namespace Core.Security.JWT;

public class TokenOptions
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
 
    public int AccessTokenExpiration { get; set; }

    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public int RefreshTokenTTL { get; set; }

    public TokenOptions()
    {
        Audience = string.Empty;
        Issuer = string.Empty;
        PrivateKey = string.Empty;
        PublicKey = string.Empty;
    }
    
}