namespace Core.Redis.MediaR;

public class JwtRemoveRedisCachableRequest : IJwtRemoveRedisCachableRequest
{
    public string Jwt { get; set; }
    public string UserId { get; set; } 
    public bool IsDeletedUserAll { get; set; }
}