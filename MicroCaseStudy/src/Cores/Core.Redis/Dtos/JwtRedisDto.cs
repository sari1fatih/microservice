namespace Core.Redis.Dtos;

public class JwtRedisDto
{
    public string UserId { get; set; }
    public List<JwtExpireDateDto>  JwtExpireDateDtos { get; set; }=new List<JwtExpireDateDto>();
}

