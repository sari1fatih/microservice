namespace Core.Redis.MediaR;
 
public interface IJwtRemoveRedisCachableRequest
{
    public string Jwt { get; set; }
    public string UserId { get; set; }
    public bool IsDeletedUserAll { get; set; }
}