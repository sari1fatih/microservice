namespace Core.Redis.MediaR;

public class JwtAddRedisCachableRequest : IJwtAddRedisCachableRequest
{
    public string Jwt { get; set; }
    public string UserId { get; set; }
    public DateTime ExpiresDate { get; set; }
}